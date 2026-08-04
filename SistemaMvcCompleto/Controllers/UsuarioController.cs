using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaCompleto.Models;
using SistemaCompleto.TDO;
using SistemaMvcCompleto.TDO;

[Authorize(Roles = "Admin")] 
public class UsuarioController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsuarioController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    #region
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Cadastrar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(RegistrarUsuarioDto model)
    {
        if (model.TipoAcesso == "Temporario" && !model.DataExpiracao.HasValue)
        {
            ModelState.AddModelError("DataExpiracao", "Informe a data de expiração para o acesso temporário.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var novoUsuario = new User
        {
            UserName = model.Email,
            Email = model.Email,
            NomeCompleto = model.NomeCompleto,
            EmailConfirmed = true
        };

        var resultado = await _userManager.CreateAsync(novoUsuario, model.Senha);

        if (resultado.Succeeded)
        {
            string roleFinal = model.TipoAcesso == "Admin" ? "Admin" : "Usuario";

            if (!await _roleManager.RoleExistsAsync(roleFinal))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleFinal));
            }

            await _userManager.AddToRoleAsync(novoUsuario, roleFinal);

            if (model.TipoAcesso == "Temporario" && model.DataExpiracao.HasValue)
            {
                await _userManager.SetLockoutEnabledAsync(novoUsuario, true);
                DateTimeOffset dataBloqueio = new DateTimeOffset(model.DataExpiracao.Value.ToUniversalTime());
                await _userManager.SetLockoutEndDateAsync(novoUsuario, dataBloqueio);
            }

            TempData["MensagemSucesso"] = $"Usuário {novoUsuario.NomeCompleto} cadastrado com sucesso!";
            return RedirectToAction("ListaFuncionarios");
        }

        foreach (var erro in resultado.Errors)
        {
            ModelState.AddModelError(string.Empty, erro.Description);
        }

        return View(model);
    }

    #endregion

    #region 📋 Listagem de Funcionários

    [HttpGet]
public async Task<IActionResult> ListaFuncionarios(string? pesquisa)
{
    var usuarioLogadoId = _userManager.GetUserId(User);

    var query = _userManager.Users
        .Where(u => u.Id != usuarioLogadoId && u.Email != "admin@sistema.com");

    if (!string.IsNullOrWhiteSpace(pesquisa))
    {
        query = query.Where(u => u.NomeCompleto.Contains(pesquisa) || u.Email!.Contains(pesquisa));
    }

    var usuarios = await query.ToListAsync();
    var listaFuncionarios = new List<FuncionarioDto>();

    foreach (var user in usuarios)
    {
        var roles = await _userManager.GetRolesAsync(user);
        string rolePrincipal = roles.FirstOrDefault() ?? "Usuario";

        bool eTemporario = user.LockoutEnabled && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow;

        listaFuncionarios.Add(new FuncionarioDto
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            Email = user.Email ?? string.Empty,

            CargoRole = rolePrincipal,
            FotoUrl = user.FotoUrl,
            IsTemporario = eTemporario,
            LockoutEnd = user.LockoutEnd
        });
    }

    ViewData["FiltroAtual"] = pesquisa;
    return View(listaFuncionarios);
}
    #endregion

    #region 

    [HttpGet]
    public async Task<IActionResult> Permissoes(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound("ID de usuário inválido.");
        }

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        var roles = await _userManager.GetRolesAsync(usuario);
        string roleAtual = roles.FirstOrDefault() ?? "Usuario";

        bool eTemporario = usuario.LockoutEnabled && usuario.LockoutEnd.HasValue && usuario.LockoutEnd > DateTimeOffset.UtcNow;

        var model = new EditarPermissoesDto
        {
            UsuarioId = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email ?? string.Empty,
            TipoAcesso = eTemporario ? "Temporario" : roleAtual,
            DataExpiracao = eTemporario ? usuario.LockoutEnd?.LocalDateTime : null
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Permissoes(EditarPermissoesDto model)
    {
        var usuario = await _userManager.FindByIdAsync(model.UsuarioId);
        if (usuario == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        if (model.TipoAcesso == "Temporario" && !model.DataExpiracao.HasValue)
        {
            ModelState.AddModelError("DataExpiracao", "Defina a data de expiração para acessos temporários.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var rolesAtuais = await _userManager.GetRolesAsync(usuario);
        await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);

        string novaRole = model.TipoAcesso == "Admin" ? "Admin" : "Usuario";
        if (!await _roleManager.RoleExistsAsync(novaRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(novaRole));
        }
        await _userManager.AddToRoleAsync(usuario, novaRole);

        if (model.TipoAcesso == "Temporario" && model.DataExpiracao.HasValue)
        {
            await _userManager.SetLockoutEnabledAsync(usuario, true);
            DateTimeOffset dataBloqueio = new DateTimeOffset(model.DataExpiracao.Value.ToUniversalTime());
            await _userManager.SetLockoutEndDateAsync(usuario, dataBloqueio);
        }
        else
        {
            await _userManager.SetLockoutEndDateAsync(usuario, null);
            await _userManager.SetLockoutEnabledAsync(usuario, false);
        }

        TempData["MensagemSucesso"] = $"Permissões do usuário {usuario.NomeCompleto} atualizadas com sucesso!";
        return RedirectToAction("ListaFuncionarios");
    }

    #region 

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            TempData["MensagemErro"] = "ID de usuário inválido.";
            return RedirectToAction(nameof(ListaFuncionarios));
        }

        var usuarioLogadoId = _userManager.GetUserId(User);
        if (usuarioLogadoId == id)
        {
            TempData["MensagemErro"] = "Você não pode excluir a sua própria conta enquanto estiver logado!";
            return RedirectToAction(nameof(ListaFuncionarios));
        }

        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null)
        {
            TempData["MensagemErro"] = "Usuário não encontrado na base de dados.";
            return RedirectToAction(nameof(ListaFuncionarios));
        }

        if (usuario.Email == "admin@sistema.com")
        {
            TempData["MensagemErro"] = "O Administrador Principal do sistema não pode ser excluído!";
            return RedirectToAction(nameof(ListaFuncionarios));
        }

        var resultado = await _userManager.DeleteAsync(usuario);

        if (resultado.Succeeded)
        {
            TempData["MensagemSucesso"] = $"Usuário {usuario.NomeCompleto} foi excluído com sucesso!";
        }
        else
        {
            var erros = string.Join(", ", resultado.Errors.Select(e => e.Description));
            TempData["MensagemErro"] = $"Falha ao excluir o usuário: {erros}";
        }

        return RedirectToAction(nameof(ListaFuncionarios));
    }

    #endregion

    #endregion
}
