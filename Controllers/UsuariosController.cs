using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using EXPEDITEE_REST.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.Globalization;

namespace EXPEDITEE_REST.Controllers;

[Route("Usuarios/[action]")]
public class UsuariosController : Controller
{

    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;

    public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment)
    {
        this.configuration = configuration;
        this.environment = environment;
    }

    public ActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    public IActionResult Create(Usuario u)
    {
        RepositorioUsuario repositorioUsuario = new RepositorioUsuario();

        Usuario usuario = repositorioUsuario.ObtenerPorEmail(u.Mail);

        if (usuario != null)
        {
            TempData["Error"] = "ya hay un usuario con este email:" + " " + u.Mail;
            return RedirectToAction("Create");
        }

        try
        {

            string salt = "palabrasecretaparalacontrasenia";
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                             password: u.Clave,
                             salt: System.Text.Encoding.ASCII.GetBytes(salt),
                             prf: KeyDerivationPrf.HMACSHA1,
                             iterationCount: 30000,
                             numBytesRequested: 256 / 8));
            u.Clave = hashed;
            u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)enRoles.Empleado;



            if (u.ImagenFile == null || u.ImagenFile.Length == 0)
            {
                u.Imagen = "/Imagenes/avatar_por_defecto.jpg";
                repositorioUsuario.Alta(u);
                return RedirectToAction("Create");
            }
            else
            {
                string wwwPath = environment.WebRootPath;
                string path = Path.Combine(wwwPath, "Uploads");

                string fileName = "avatar_" + Guid.NewGuid().ToString("N") + Path.GetExtension(u.ImagenFile.FileName);
                string pathCompleto = Path.Combine(path, fileName);
                u.Imagen = Path.Combine("/Uploads", fileName);

                int res = repositorioUsuario.Alta(u);
                if (res > 0)
                {
                    // Esta operación guarda la foto en memoria en la ruta que necesitamos
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.ImagenFile.CopyTo(stream);
                    }
                    return RedirectToAction("Create");
                }

            }


        }
        catch (Exception ex)
        {
            TempData["Error"] = "error al cargar el usuario, asegurese de llenar todos los campos obligatorios";
            return RedirectToAction("Create");
        }

        return View();
    }


    [AllowAnonymous]
    // GET: Usuarios/Login/
    public ActionResult Login(string returnUrl)
    {
        try
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

    // POST: Usuarios/Login/
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginView login)
    {
        RepositorioUsuario repositorio = new RepositorioUsuario();
        try
        {
            var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();

            if (ModelState.IsValid)
            {
                var e = repositorio.ObtenerPorEmail(login.Mail);

                if (e == null)
                {
                    ModelState.AddModelError("", "El email o la clave no son correctos");
                    TempData["returnUrl"] = returnUrl;
                    return View();
                }

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                 password: login.Clave,
                                 salt: System.Text.Encoding.ASCII.GetBytes("palabrasecretaparalacontrasenia"),
                                 prf: KeyDerivationPrf.HMACSHA1,
                                 iterationCount: 30000,
                                 numBytesRequested: 256 / 8));

                if (e.Clave != hashed)
                {
                    ModelState.AddModelError("", "El email o la clave no son correctos");
                    TempData["returnUrl"] = returnUrl;
                    return View();
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, e.Mail),
                new Claim("FullName", e.Nombre + " " + e.Apellido),
                new Claim(ClaimTypes.Role, e.RolNombre),
                new Claim("EmpleadoId", e.Id.ToString()),

            };

                var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                TempData.Remove("returnUrl");
                return Redirect(returnUrl);
            }
            TempData["returnUrl"] = returnUrl;
            return View();
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }



    // GET: /salir
    [Route("salir", Name = "logout")]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }



}