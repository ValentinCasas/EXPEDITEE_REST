using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

public class ListaFavoritos
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public List<Producto> Productos { get; set; }
    public Usuario Usuario { get; set; }
}
