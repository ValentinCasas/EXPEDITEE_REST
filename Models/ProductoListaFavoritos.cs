using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EXPEDITEE_REST.Models
{
    public class ProductoListaFavoritos
    {

        public int Id { get; set; }

        public int IdListaFavoritos { get; set; }
        [ForeignKey(nameof(IdListaFavoritos))]
        public ListaFavoritos ListaFavoritos { get; set; }

        public int IdProducto { get; set; }
        [ForeignKey(nameof(IdProducto))]
        public Producto Producto { get; set; }
        
        public int Cantidad { get; set; }

    }

}