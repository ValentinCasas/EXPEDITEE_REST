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
public class MensajeController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment environment;

    public MensajeController(DataContext context, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        this.environment = environment;
    }

    // Envío de mensaje a empleado
    [HttpGet("enviar/{idEmisor}/{idReceptor}/{mensaje}")]
    public Mensaje enviarMensaje(int idEmisor, int idReceptor, string mensaje)
    {
        Usuario emisor = _context.Usuario.FirstOrDefault(u => u.Id == idEmisor);
        Usuario receptor = _context.Usuario.FirstOrDefault(u => u.Id == idReceptor);

        if (emisor != null && receptor != null)
        {
            Mensaje msj = new Mensaje
            {
                Descripcion = mensaje,
                Fecha = DateTime.Now,
                Emisor = emisor,
                Receptor = receptor
            };

            _context.Mensaje.Add(msj);
            _context.SaveChanges();

            return msj;
        }

        return null;
    }



    // Obtener mensajes por idEmisor y idReceptor
    [HttpGet("mensajes/{idEmisor}/{idReceptor}")]
    public List<Mensaje> ObtenerMensajes(int idEmisor, int idReceptor)
    {
        var mensajes = _context.Mensaje
            .Include(m => m.Emisor)
            .Include(m => m.Receptor)
            .Where(m => m.IdEmisor == idEmisor && m.IdReceptor == idReceptor || m.IdEmisor == idReceptor && m.IdReceptor == idEmisor)
            .ToList();

        return mensajes;
    }





}