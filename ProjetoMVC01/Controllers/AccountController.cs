using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProjetoMVC01.Models;
using ProjetoMVC01.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjetoMVC01.Controllers
{
    public class AccountController : Controller
    {
        //método que abre a página de login..
        public IActionResult Login()
        {
            return View();
        }

        //método que recebe o submit POST do formulário
        [HttpPost]
        public IActionResult Login(LoginModel model, [FromServices] UsuarioRepository usuarioRepository)
        {
            //verificar se os campos da model (email e senha)
            //passaram nas regras de validação..
            if (ModelState.IsValid)
            {
                try
                {
                    //buscar o usuario no banco de dados baseado no id..
                    var usuario = usuarioRepository.Consultar(model.Email, model.Senha);

                    //verificar se o usuario foi encontrado..
                    if (usuario != null)
                    {
                        //gravar o COOKIE de autenticação do usuário..
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, usuario.Email) }, CookieAuthenticationDefaults.AuthenticationScheme);

                        //autenticando o usuario no NET CORE MVC!
                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        //redirecionar para a página /Home/Index
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Mensagem"] = "Acesso Negado. Usuário inválido.";
                    }
                }
                catch (Exception e)
                {
                    TempData["Mensagem"] = "Erro: " + e.Message;
                }
            }

            return View();
        }

        //método para sair do sistema
        public IActionResult Logout()
        {
            //apagar o cookie de autenticação do usuário..
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //redirecionar para a página de login..
            return RedirectToAction("Login", "Account");
        }

    }
}

