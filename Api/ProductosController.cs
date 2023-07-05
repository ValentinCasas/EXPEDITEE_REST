using QRCoder;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EXPEDITEE_REST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Drawing;
using System.IO;
using System.Net;
using ZXing.QrCode.Internal;
using MercadoPago;
using System.Drawing.Imaging;
using MailKit.Net.Smtp;
using MimeKit;
using System.Security.Authentication;

namespace EXPEDITEE_REST.Api;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment environment;

    public ProductosController(DataContext context, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        this.environment = environment;
    }



    //se muestra la cantidad y el id del producto del pedido
    //uso estos id para que me devuelva los productos aca obtenerProductosPorPedido v

    /*      "id": 5,
            "idPedido": 3,
            "pedido": null,
            "idProducto": 9,
            "producto": null,
            "cantidad": 1,
            "monto": 3000       */

    [HttpGet("obtenerProductosPorPedido/{id:int:min(1)}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public List<ProductoPedido> obtenerProductosPorPedido(int id)
    {
        var productosPedido = _context.ProductoPedido
        .Where(pp => pp.IdPedido == id)
        /* .Select(pp => pp.Producto) */
        .ToList();
        return productosPedido;
    }


    // http://192.168.0.102:5250/api/Usuario/obtenerProductosPorIds?ids=9&ids=10

                    /* "id": 9,
                    "cantidad": 100,
                    "categoria": "comida y bebida",
                    "descripcion": "super combo de bebida y comida",
                    "imagen": "/Img_productos\\avatar_4d0f984e6d834248b569f35ac22148a9.jpg",
                    "imagenFile": null,
                    "nombre": "super combo",
                    "precio": 3000          */

    // recibe array de ids de productos y devuelve los productos
    [HttpGet("obtenerProductosPorIds")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public List<Producto> obtenerProductosPorIds([FromQuery] int[] ids)
    {
        var productos = _context.Producto
            .Where(p => ids.Contains(p.Id))
            .ToList();

        return productos;
    }



}