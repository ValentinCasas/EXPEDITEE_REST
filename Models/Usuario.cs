using EXPEDITEE_REST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EXPEDITEE_REST.Models
{
    public enum enRoles
        {

            Administrador = 1,
            Empleado = 2,
            Cliente = 3
        }


    public class Usuario
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long Dni { get; set; }
        public string Mail { get; set; }

        [Required, DataType(DataType.Password)]
        public string Clave { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }

        public string Imagen { get; set; }
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }


        public int Rol { get; set; }
        [NotMapped]
        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

        public static IDictionary<int, string> ObtenerRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoEnumRol = typeof(enRoles);
            foreach (var valor in Enum.GetValues(tipoEnumRol))
            {
                roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
            }
            return roles;
        }


    }

}