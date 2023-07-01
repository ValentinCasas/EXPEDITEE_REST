using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EXPEDITEE_REST.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
      
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<PedidoListaFavoritos> PedidoListaFavoritos { get; set; }

        public DbSet<ProductoListaFavoritos> ProductoListaFavoritos { get; set; }
        public DbSet<ListaFavoritos> ListaFavoritos { get; set; }
        public DbSet<ProductoPedido> ProductoPedido { get; set; }

        public DbSet<Retroalimentacion> Retroalimentacion { get; set;}
        public DbSet<Mensaje> Mensaje { get; set;}
        public DbSet<ComprobanteEfectivo> ComprobanteEfectivo { get; set;}


    }
}
