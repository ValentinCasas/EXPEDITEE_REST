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
public class PedidoController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment environment;

    public PedidoController(DataContext context, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        this.environment = environment;
    }


    [HttpGet("actualizarEstadoPedido/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public void ActualizarEstadoPedido(int id)
    {
        var pedido = _context.Pedido.FirstOrDefault(x => x.Id == id);

        pedido.Estado = "entregado"; 

        _context.SaveChanges();


    }



    //devuelve el pedido segun su id
    /* "estado": "pendiente",
    "fecha": "2023-06-22T00:00:00",
    "id": 3,
    "idEmpleado": 13,
    "empleado": null,
    "idCliente": 17,
    "cliente": null,
    "montoTotal": 22718 */
    [HttpGet("obtenerPedidoPorId/{id:int:min(1)}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public Pedido ObtenerPedidoPorId(int id)
    {

        Pedido pedido = _context.Pedido.FirstOrDefault(p => p.Id == id);

        return pedido;
    }




}