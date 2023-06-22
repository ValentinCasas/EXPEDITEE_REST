using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

public class VotoRetroalimentacion
{
    public int Id { get; set; }
    public Retroalimentacion Retroalimentacion { get; set; }
    public Usuario Usuario { get; set; }
}
