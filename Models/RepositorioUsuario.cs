using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace EXPEDITEE_REST.Models
{
    public class RepositorioUsuario
    {

        string connectionString = "Server=localhost;User=root;Password=;Database=expeditee;SslMode=none";


        public RepositorioUsuario()
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


        public int Alta(Usuario usuario)
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO usuario (Nombre, Apellido, Dni, Mail, Clave, Telefono, Direccion, Ciudad, Pais, Latitud, Longitud, Imagen, Rol)
                        VALUES
                        (@nombre, @apellido, @dni, @mail, @clave, @telefono, @direccion, @ciudad, @pais, @latitud, @longitud, @imagen, @rol);
                        SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@dni", usuario.Dni);
                    command.Parameters.AddWithValue("@mail", usuario.Mail);
                    command.Parameters.AddWithValue("@clave", usuario.Clave);
                    command.Parameters.AddWithValue("@telefono", usuario.Telefono);
                    command.Parameters.AddWithValue("@direccion", usuario.Direccion);
                    command.Parameters.AddWithValue("@ciudad", usuario.Ciudad);
                    command.Parameters.AddWithValue("@pais", usuario.Pais);
                    command.Parameters.AddWithValue("@latitud", usuario.Latitud);
                    command.Parameters.AddWithValue("@longitud", usuario.Longitud);
                    command.Parameters.AddWithValue("@imagen", usuario.Imagen);
                    command.Parameters.AddWithValue("@rol", usuario.Rol);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    usuario.Id = res;
                    connection.Close();
                }
            }
            return res;
        }


        public Usuario ObtenerPorEmail(string mail)
        {
            Usuario? usuario = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT
            Id, Nombre, Apellido, Dni, Mail, Clave, Telefono, Direccion, Ciudad, Pais, Latitud, Longitud, Imagen, Rol
            FROM usuario
            WHERE Mail=@mail";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@mail", mail);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario();
                            usuario.Id = reader.GetInt32("Id");
                            usuario.Nombre = reader.GetString("Nombre");
                            usuario.Apellido = reader.GetString("Apellido");
                            usuario.Dni = reader.GetInt64("Dni");
                            usuario.Mail = reader.GetString("Mail");
                            usuario.Clave = reader.GetString("Clave");
                            usuario.Telefono = reader.GetInt64("Telefono");
                            usuario.Direccion = reader.GetString("Direccion");
                            usuario.Ciudad = reader.GetString("Ciudad");
                            usuario.Pais = reader.GetString("Pais");
                            usuario.Latitud = reader.GetString("Latitud");
                            usuario.Longitud = reader.GetString("Longitud");
                            usuario.Imagen = reader.GetString("Imagen");
                            usuario.Rol = reader.GetInt32("Rol");

                            return usuario;
                        }
                    }
                }
            }
            return usuario;
        }




    }

}
