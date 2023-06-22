using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

public class ComprobanteEfectivo
{
    public int Id { get; set; }
    public string Imagen { get; set; }
    public Pedido Pedido { get; set; }
}
