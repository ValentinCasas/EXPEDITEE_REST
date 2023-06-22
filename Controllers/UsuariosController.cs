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

}