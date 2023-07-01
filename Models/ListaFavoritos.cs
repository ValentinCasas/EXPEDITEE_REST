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
    public class ListaFavoritos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public int IdCliente { get; set; }
        [ForeignKey(nameof(IdCliente))]
        public Usuario Usuario { get; set; }




    }
}