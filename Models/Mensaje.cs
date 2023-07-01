using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Linq;
using System.Threading.Tasks;

public class Mensaje
{
    public string Descripcion { get; set; }

    public int IdEmisor { get; set; }
    [ForeignKey(nameof(IdEmisor))]
    public Usuario Emisor { get; set; }
    
    public DateTime Fecha { get; set; }
    public int Id { get; set; }

    public int IdReceptor { get; set; }
    [ForeignKey(nameof(IdReceptor))]
    public Usuario Receptor { get; set; }
}
