using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

public class Mensaje
{
    public string Descripcion { get; set; }
    public Usuario Emisor { get; set; }
    public DateTime Fecha { get; set; }
    public int Id { get; set; }
    public Usuario Receptor { get; set; }
}
