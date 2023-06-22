using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EXPEDITEE_REST.Models
{
public class Retroalimentacion
{
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaEnvio{ get; set; }

    public int IdUsuario { get; set; }
    [ForeignKey(nameof(IdUsuario))]
    public Usuario Usuario { get; set; }
}

}