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

[Route("Productos/[action]")]
public class ProductosController : Controller
{

    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;

    public ProductosController(IConfiguration configuration, IWebHostEnvironment environment)
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
    public IActionResult Create(Producto p)
    {
        RepositorioProducto repositorioProducto = new RepositorioProducto();

        bool hayProducto = repositorioProducto.HayProductoPorNombre(p.Nombre);

        if (hayProducto)
        {
            //sumarle la cantidad de productos a producto.cantidad
            repositorioProducto.ActualizarCantidad(p.Nombre, p.Cantidad);

        }
        else
        {
            try
            {
                if (p.ImagenFile == null || p.ImagenFile.Length == 0)
                {
                    p.Imagen = "/Imagenes/producto_por_defecto.jpg";
                    repositorioProducto.Alta(p);
                    return RedirectToAction("Create");
                }
                else
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Img_productos");

                    string fileName = "avatar_" + Guid.NewGuid().ToString("N") + Path.GetExtension(p.ImagenFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    p.Imagen = Path.Combine("/Img_productos", fileName);

                    int res = repositorioProducto.Alta(p);
                    if (res > 0)
                    {
                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            p.ImagenFile.CopyTo(stream);
                        }
                        return RedirectToAction("Create");
                    }

                }


            }
            catch (Exception ex)
            {
                TempData["Error"] = "error al cargar el producto";
                return RedirectToAction("Create");
            }

        }

        return View();
    }



}
