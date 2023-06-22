using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EXPEDITEE_REST.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public string Categoria { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
    }


}