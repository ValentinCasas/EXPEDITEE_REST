using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EXPEDITEE_REST.Models
{

    public class UsuarioIdPedido
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long Dni { get; set; }
        public string Mail { get; set; }

        public string Clave { get; set; }
        public long Telefono { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }

        public string Imagen { get; set; }

        public int? IdPedido { get; set; }

        public int Rol { get; set; }




    }

}