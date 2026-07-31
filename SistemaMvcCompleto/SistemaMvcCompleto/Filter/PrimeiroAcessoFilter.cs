using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using SistemaCompleto.Models;

namespace SistemaCompleto.Filters
{
    public class PrimeiroAcessoFilter : IAsyncActionFilter
    {
        private readonly UserManager<User> _userManager;

        public PrimeiroAcessoFilter(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userPrincipal = context.HttpContext.User;

            if (userPrincipal.Identity != null && userPrincipal.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(userPrincipal);

                if (user != null && user.PrimeiroAcesso)
                {
                    var controllerName = context.RouteData.Values["controller"]?.ToString();
                    var actionName = context.RouteData.Values["action"]?.ToString();

                    // 🛡️ Comparações case-insensitive garantem match independente da formatação da URL
                    bool isAccountController = string.Equals(controllerName, "Account", StringComparison.OrdinalIgnoreCase);
                    bool isAlterarSenhaAction = string.Equals(actionName, "AlterarSenhaPrimeiroAcesso", StringComparison.OrdinalIgnoreCase);
                    bool isLogoutAction = string.Equals(actionName, "Logout", StringComparison.OrdinalIgnoreCase);

                    // Libera apenas as rotas de Logout e de Alterar Senha para evitar loop de redirecionamento
                    bool isAllowedAction = isAccountController && (isAlterarSenhaAction || isLogoutAction);

                    if (!isAllowedAction)
                    {
                        context.Result = new RedirectToActionResult("AlterarSenhaPrimeiroAcesso", "Account", null);
                        return;
                    }
                }
            }

            await next();
        }
    }
}