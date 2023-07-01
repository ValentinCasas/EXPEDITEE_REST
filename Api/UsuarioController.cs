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
public class UsuarioController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment environment;

    public UsuarioController(DataContext context, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        this.environment = environment;
    }


    //POST api/<controller>/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginView loginView)
    {
        try
        {
            var usuario = _context.Usuario.FirstOrDefault(x => x.Mail == loginView.Mail);
            if (usuario == null)
            {
                return NotFound();
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: loginView.Clave,
                salt: System.Text.Encoding.ASCII.GetBytes("palabrasecretaparalacontrasenia"),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 30000,
                numBytesRequested: 256 / 8));
            var p = await _context.Usuario.FirstOrDefaultAsync(x => x.Mail == loginView.Mail);
            if (p == null || p.Clave != hashed)
            {
                return BadRequest("Nombre de usuario o clave incorrecta");
            }
            else
            {
                var key = new SymmetricSecurityKey(
                    System.Text.Encoding.ASCII.GetBytes("%303rOMgzzsvPnGw$lZG7llN@C%rd^M1nyxJvtwMyaB2TNWDMq"));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, p.Mail),
                    new Claim("FullName", p.Nombre + " " + p.Apellido),
                    new Claim(ClaimTypes.Role, "Empleado"),
                };

                var token = new JwtSecurityToken(
                    issuer: _configuration["TokenAuthentication:Issuer"],
                    audience: _configuration["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credenciales
                );
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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

    // GET api/propietario/data
    [HttpGet("data")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetUsuarioActual()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            var emailClaim = identity.FindFirst(ClaimTypes.Name);
            var fullNameClaim = identity.FindFirst("FullName");
            var roleClaim = identity.FindFirst(ClaimTypes.Role);

            var email = emailClaim?.Value;
            var fullName = fullNameClaim?.Value;
            var role = roleClaim?.Value;

            var propietario = _context.Usuario.FirstOrDefault(x => x.Mail == email);

            if (propietario != null)
            {
                return Ok(propietario);
            }
        }

        return Unauthorized();
    }


    // GET api/propietario/imagen/{id}
    [HttpGet("imagen/{id:int:min(1)}")]
    public IActionResult ObtenerImagenPropietario(int id)
    {
        var propietario = _context.Usuario.FirstOrDefault(x => x.Id == id);
        if (propietario == null)
        {
            return NotFound();
        }

        var wwwPath = environment.WebRootPath;
        var ruta = Path.Combine(wwwPath, propietario.Imagen.TrimStart('/').Replace('/', '\\'));

        if (ruta.StartsWith(Path.Combine(wwwPath, "Uploads")))
        {
            if (System.IO.File.Exists(ruta))
            {
                var fileBytes = System.IO.File.ReadAllBytes(ruta);
                return File(fileBytes, "image/jpeg");
            }
        }
        else
        {
            // Ruta por defecto para la imagen
            var defaultImagePath = Path.Combine(wwwPath, "imagenes", "avatar_por_defecto.jpg");
            if (System.IO.File.Exists(defaultImagePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(defaultImagePath);
                return File(fileBytes, "image/jpeg");
            }
        }

        return NotFound();
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



    [HttpGet("usuarioPorId/{id}")]
    public Usuario getUsuario(int id)
    {
        var usuario = _context.Usuario.FirstOrDefault(x => x.Id == id);
        return usuario;
    }


    [HttpGet("actualizarEstadoPedido/{id}")]
    public void ActualizarEstadoPedido(int id)
    {
        var pedido = _context.Pedido.FirstOrDefault(x => x.Id == id);

        pedido.Estado = "entregado"; // Actualizar el estado del pedido

        _context.SaveChanges(); // Guardar los cambios en la base de datos


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


    //generarImagenQr
    /*  [HttpGet("generarQr/{montoTotal}")]
    public IActionResult generarQr(decimal montoTotal)
    {
        string textoQr = $"https://www.mercadopago.com/pay?total={montoTotal}";

        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(textoQr, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);

        Bitmap qrCodeImage = qrCode.GetGraphic(10);

        using (var ms = new MemoryStream())
        {
            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] qrImageBytes = ms.ToArray();
            return File(qrImageBytes, "image/png");
        }
    }  */


    [HttpGet("generarImgComprobante/{idPedido}/{montoTotal}/{nombreEmpleado}/{nombreCliente}/{mail}")]
    public async Task<IActionResult> generarImgComprobante(int idPedido, decimal montoTotal, string nombreEmpleado, string nombreCliente, string mail)
    {
        string expediteeText = "EXPEDITEE";
        string cobroExitosoText = "Cobro exitoso";
        string empleadoText = $"Empleado: {nombreEmpleado}";
        string clienteText = $"Cliente: {nombreCliente}";
        string fechaCobroText = $"Fecha cobro: {DateTime.Now.ToString()}";
        string montoText = $"Monto: {montoTotal}";

        int width = 400; // Ancho de la imagen
        int height = 300; // Alto de la imagen

        using (Bitmap bitmap = new Bitmap(width, height))
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Fondo verde
                Color backgroundColor = ColorTranslator.FromHtml("#2CBD60");
                graphics.Clear(backgroundColor);

                // Fuente y tamaño para "EXPEDITEE"
                Font expediteeFont = new Font("Arial", 24, FontStyle.Bold);
                SizeF expediteeSize = graphics.MeasureString(expediteeText, expediteeFont);
                PointF expediteePosition = new PointF((width - expediteeSize.Width) / 2, 50);
                graphics.DrawString(expediteeText, expediteeFont, Brushes.White, expediteePosition);

                // Fuente y tamaño para "Cobro exitoso"
                Font cobroExitosoFont = new Font("Arial", 18, FontStyle.Regular);
                SizeF cobroExitosoSize = graphics.MeasureString(cobroExitosoText, cobroExitosoFont);
                PointF cobroExitosoPosition = new PointF((width - cobroExitosoSize.Width) / 2, 100);
                graphics.DrawString(cobroExitosoText, cobroExitosoFont, Brushes.White, cobroExitosoPosition);

                // Fuente y tamaño para el nombre del empleado
                Font empleadoFont = new Font("Arial", 12, FontStyle.Regular);
                SizeF empleadoSize = graphics.MeasureString(empleadoText, empleadoFont);
                PointF empleadoPosition = new PointF((width - empleadoSize.Width) / 2, 150);
                graphics.DrawString(empleadoText, empleadoFont, Brushes.White, empleadoPosition);

                // Fuente y tamaño para el nombre del cliente
                Font clienteFont = new Font("Arial", 12, FontStyle.Regular);
                SizeF clienteSize = graphics.MeasureString(clienteText, clienteFont);
                PointF clientePosition = new PointF((width - clienteSize.Width) / 2, 170);
                graphics.DrawString(clienteText, clienteFont, Brushes.White, clientePosition);

                // Fuente y tamaño para la fecha de cobro
                Font fechaCobroFont = new Font("Arial", 12, FontStyle.Regular);
                SizeF fechaCobroSize = graphics.MeasureString(fechaCobroText, fechaCobroFont);
                PointF fechaCobroPosition = new PointF((width - fechaCobroSize.Width) / 2, 190);
                graphics.DrawString(fechaCobroText, fechaCobroFont, Brushes.White, fechaCobroPosition);

                // Fuente y tamaño para el monto
                Font montoFont = new Font("Arial", 12, FontStyle.Regular);
                SizeF montoSize = graphics.MeasureString(montoText, montoFont);
                PointF montoPosition = new PointF((width - montoSize.Width) / 2, 210);
                graphics.DrawString(montoText, montoFont, Brushes.White, montoPosition);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();


                string wwwPath = environment.WebRootPath;
                string path = Path.Combine(wwwPath, "comprobantes");

                string fileName = "comprobante_" + Guid.NewGuid().ToString("N") + ".png";
                string pathCompleto = Path.Combine(path, fileName);

                // Guardar la imagen en el sistema de archivos
                using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                {
                    await stream.WriteAsync(imageBytes, 0, imageBytes.Length);
                }

                ComprobanteEfectivo comprobante = new ComprobanteEfectivo
                {
                    IdPedido = idPedido,
                    Imagen = Path.Combine("/comprobantes", fileName)
                };

                _context.ComprobanteEfectivo.Add(comprobante);
                _context.SaveChanges();


                SendEmailWithImage(mail, imageBytes);


                return File(imageBytes, "image/png");
            }
        }
    }

    // Función para enviar el correo electrónico con la imagen adjunta
    private void SendEmailWithImage(string recipient, byte[] imageBytes)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("EXPEDITEE EMPRESA", "valen.casas12@gmail.com")); // Remitente
        message.To.Add(new MailboxAddress("", recipient)); // Destinatario
        message.Subject = "Comprobante de pago";

        var builder = new BodyBuilder();
        builder.HtmlBody = "<p>Adjunto se encuentra el comprobante de pago.</p>";

        // Adjuntar la imagen al mensaje
        builder.Attachments.Add("comprobante.png", imageBytes, ContentType.Parse("image/png"));

        message.Body = builder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            client.Connect("smtp.gmail.com", 587, false); // Configurar el servidor SMTP y el puerto
            client.Authenticate("valen.casas12@gmail.com", "xuvikaynyezunekf"); // Configurar las credenciales de autenticación

            // client.SslProtocols = SslProtocols.Tls;

            client.Send(message); // Enviar el mensaje
            client.Disconnect(true); // Desconectar el cliente SMTP
        }
    }



    //devuelve todos los empleados
    [HttpGet("empleados")]
    public List<Usuario> ObtenerEmpleados()
    {
        var empleados = _context.Usuario
        .Where(u => u.Rol == 2)
        .ToList();
        return empleados;
    }

    //devuelve los clientes con el id del pedido que hizo    
    [HttpGet("misPendientes/{id:int:min(1)}")]
    public List<UsuarioIdPedido> ObtenerClientesConPedidosPorEmpleado(int id)
    {
        var pedidos = _context.Pedido
            .Where(p => p.IdEmpleado == id && p.Estado != "entregado")
            .Include(p => p.Cliente)
            .Include(p => p.Empleado)
            // Incluir otras entidades relacionadas si existen
            .ToList();

        var usuarios = pedidos.Select(p => new UsuarioIdPedido
        {
            Id = p.Cliente.Id,
            Nombre = p.Cliente.Nombre,
            Apellido = p.Cliente.Apellido,
            Dni = p.Cliente.Dni,
            Mail = p.Cliente.Mail,
            Clave = p.Cliente.Clave,
            Telefono = p.Cliente.Telefono,
            Direccion = p.Cliente.Direccion,
            Ciudad = p.Cliente.Ciudad,
            Pais = p.Cliente.Pais,
            Latitud = p.Cliente.Latitud,
            Longitud = p.Cliente.Longitud,
            Imagen = p.Cliente.Imagen,
            IdPedido = p.Id,
            Rol = p.Cliente.Rol
        }).ToList();

        return usuarios;
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
    public Pedido ObtenerPedidoPorId(int id)
    {

        Pedido pedido = _context.Pedido.FirstOrDefault(p => p.Id == id);

        return pedido;
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
    "precio": 3000 */

    // recibe array de ids de productos y devuelve los productos
    [HttpGet("obtenerProductosPorIds")]
    public List<Producto> obtenerProductosPorIds([FromQuery] int[] ids)
    {
        var productos = _context.Producto
            .Where(p => ids.Contains(p.Id))
            .ToList();

        return productos;
    }


















}