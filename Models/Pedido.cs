using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EXPEDITEE_REST.Models
{
    public class Pedido
    {
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }
        public int Id { get; set; }

        public int IdEmpleado { get; set; }
        [ForeignKey(nameof(IdEmpleado))]
        [JsonIgnore]
        public Usuario Empleado { get; set; }

        public int IdCliente { get; set; }
        [ForeignKey(nameof(IdCliente))]
        [JsonIgnore]
        public Usuario Cliente { get; set; }

        public decimal MontoTotal { get; set; }


/*         public Pedido()
        {
            ProductosPedidos = new List<ProductoPedido>();
        }

        public ICollection<ProductoPedido> ProductosPedidos { get; set; } */

    }

}