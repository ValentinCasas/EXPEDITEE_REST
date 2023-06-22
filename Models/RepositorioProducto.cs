using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace EXPEDITEE_REST.Models
{
    public class RepositorioProducto
    {
        private string connectionString = "Server=localhost;User=root;Password=;Database=expeditee;SslMode=none";

        public RepositorioProducto()
        {
        }

        public bool HayProductoPorNombre(string nombre)
        {
            bool existeProducto = false;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(*) FROM producto WHERE Nombre = @nombre";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", nombre);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    existeProducto = count > 0;
                    connection.Close();
                }
            }

            return existeProducto;
        }


        public void ActualizarCantidad(string nombre, int cantidadASumar)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"UPDATE producto
                        SET Cantidad = Cantidad + @cantidadASumar
                        WHERE Nombre = @nombre";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cantidadASumar", cantidadASumar);
                    command.Parameters.AddWithValue("@nombre", nombre);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }


        public int Alta(Producto producto)
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"INSERT INTO producto (Cantidad, Categoria, Descripcion, Imagen, Nombre, Precio)
                        VALUES
                        (@cantidad, @categoria, @descripcion, @imagen, @nombre, @precio);
                        SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cantidad", producto.Cantidad);
                    command.Parameters.AddWithValue("@categoria", producto.Categoria);
                    command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@imagen", producto.Imagen);
                    command.Parameters.AddWithValue("@nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@precio", producto.Precio);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    producto.Id = res;
                    connection.Close();
                }
            }
            return res;
        }
    }
}
