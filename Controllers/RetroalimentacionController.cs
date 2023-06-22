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

[Route("Retroalimentacion/[action]")]
public class RetroalimentacionController : Controller
{

    public RetroalimentacionController()
    {
    }


    [HttpGet]
    public IActionResult Create()
    {
        RepositorioRetroalimentacion repoRetroalimentacion = new RepositorioRetroalimentacion();
        ViewBag.usuarios = repoRetroalimentacion.GetUsuarios();
        ViewBag.Roles = Usuario.ObtenerRoles();
        return View();
    }


    [HttpPost]
    public IActionResult Create(Retroalimentacion retroalimentacion)
    {
        RepositorioRetroalimentacion repoRetroalimentacion = new RepositorioRetroalimentacion();
        retroalimentacion.FechaEnvio = DateTime.Now;
        repoRetroalimentacion.Alta(retroalimentacion);

        return View();
    }


}