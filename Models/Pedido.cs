using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

public class Pedido
{
    public string Estado { get; set; }
    public DateTime Fecha { get; set; }
    public int Id { get; set; }
    public ListaFavoritos ListaFavoritos { get; set; }
    public decimal MontoTotal { get; set; }
}
