using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace EXPEDITEE_REST.Models
{
    public class RepositorioRetroalimentacion
    {

        string connectionString = "Server=localhost;User=root;Password=;Database=expeditee;SslMode=none";


        public RepositorioRetroalimentacion()
        {
        }

        public List<Usuario> GetUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                var query = @"SELECT Id, Nombre, Apellido, Dni, Mail, Clave, Telefono, Direccion, Ciudad, Pais, Latitud, Longitud, Imagen, Rol
                      FROM usuario";
                using (var command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = new Usuario()
                            {
                                Id = reader.GetInt32("Id"),
                                Dni = reader.GetInt64("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Mail = reader.GetString("Mail"),
                                Clave = reader.GetString("Clave"),
                                Telefono = reader.GetInt64("Telefono"),
                                Direccion = reader.GetString("Direccion"),
                                Ciudad = reader.GetString("Ciudad"),
                                Pais = reader.GetString("Pais"),
                                Latitud = reader.GetString("Latitud"),
                                Longitud = reader.GetString("Longitud"),
                                Imagen = reader.GetString("Imagen"),
                                Rol = reader.GetInt32("Rol")
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            }
            return usuarios;
        }



        public int Alta(Retroalimentacion retroalimentacion)
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO retroalimentacion (FechaEnvio,Descripcion,IdUsuario)
                            VALUES
                            (@fechaEnvio, @descripcion,@idusuario);
                            SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fechaEnvio", retroalimentacion.FechaEnvio);
                    command.Parameters.AddWithValue("@descripcion", retroalimentacion.Descripcion);
                    command.Parameters.AddWithValue("@idusuario", retroalimentacion.IdUsuario);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    retroalimentacion.Id = res;
                    connection.Close();
                }
            }
            return res;
        }





    }

}
