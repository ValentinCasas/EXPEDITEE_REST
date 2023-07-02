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
public class RetroalimentacionController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment environment;

    public RetroalimentacionController(DataContext context, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        this.environment = environment;
    }


    //retroalimentaciones
    [HttpGet("retroalimentaciones")]
    public List<Retroalimentacion> getRetroalimentaciones()
    {
        var retroalimentacion = _context.Retroalimentacion
          .Include(m => m.Usuario)
          .ToList();

        return retroalimentacion;
    }


}