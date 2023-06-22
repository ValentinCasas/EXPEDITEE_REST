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

[Route("ListaFavoritosController/[action]")]
public class ListaFavoritosController : Controller
{

    public ListaFavoritosController()
    {
    }


    [HttpGet]
    public IActionResult Create()
    {
        RepositorioListaFavoritos repositorioListaFavoritos = new RepositorioListaFavoritos();
        ViewBag.usuarios = repositorioListaFavoritos.GetUsuarios();
        ViewBag.Roles = Usuario.ObtenerRoles();
        return View();
    }


    [HttpPost]
    public IActionResult Create(ListaFavoritos listaFavoritos)
    {
        RepositorioListaFavoritos repositorioListaFavoritos = new RepositorioListaFavoritos();
        repositorioListaFavoritos.Alta(listaFavoritos);

        return View();
    }


}