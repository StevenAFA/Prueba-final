using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Prueba_final.Views
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarListaTablas();
            }
        }

        private void CargarListaTablas()
        {
            string connectionString = "Server=LAPTOP-G02OT5LT;Database=CreateTables;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);

                        ListBoxTables.DataSource = dataSet;
                        ListBoxTables.DataTextField = "TABLE_NAME";
                        ListBoxTables.DataBind();
                    }
                }
            }
        }

        protected void ButtonDelete_Click(object sender, EventArgs e)
        {
            string tableName = ListBoxTables.SelectedValue;

            if (!string.IsNullOrEmpty(tableName))
            {
                string connectionString = "Server=LAPTOP-G02OT5LT;Database=CreateTables;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DROP TABLE " + tableName;

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Mostrar alerta de éxito
                    ScriptManager.RegisterStartupScript(this, GetType(), "DeleteSuccess", "alert('Tabla eliminada exitosamente.'); window.location.href = window.location.href;", true);
                }
            }
            else
            {
                // Mostrar alerta de error porque no se ha seleccionado ninguna tabla
                ScriptManager.RegisterStartupScript(this, GetType(), "NoTableSelected", "alert('Por favor, seleccione una tabla para eliminar.');", true);
            }
        }

        protected void crearTablaBtn_Click(object sender, EventArgs e)
        {
            string nombreTablaValor = nombreTabla.Text;
            string camposTexto = campos.Text;
            string[] camposValores = camposTexto.Split(',');

            string connectionString = "Server=LAPTOP-G02OT5LT;Database=CreateTables;Integrated Security=True;";

            try
            {
                if (string.IsNullOrEmpty(nombreTablaValor) || !EsAlfanumerico(nombreTablaValor))
                {
                    throw new Exception("El nombre de la tabla no puede estar vacío y debe ser alfanumérico.");
                }

                if (string.IsNullOrEmpty(camposTexto) || !EsFormatoCorrecto(camposTexto))
                {
                    throw new Exception("La lista de campos no puede estar vacía y debe estar en el formato correcto (separados por coma).");
                }

                if (TablaExiste(nombreTablaValor, connectionString))
                {
                    throw new Exception("Ya existe una tabla con el mismo nombre en la base de datos.");
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string commandText = $"CREATE TABLE {nombreTablaValor} (";

                    foreach (string campo in camposValores)
                    {
                        commandText += $"{campo.Trim()} VARCHAR(50), ";
                    }

                    commandText = commandText.TrimEnd(',', ' ') + ")";

                    using (SqlCommand command = new SqlCommand(commandText, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Mostrar alerta de éxito
                    ScriptManager.RegisterStartupScript(this, GetType(), "CreateSuccess", "alert('Tabla creada exitosamente.'); window.location.href = window.location.href;", true);
                }

                CargarListaTablas();
            }
            catch (Exception ex)
            {
                // Mostrar alerta de error
                ScriptManager.RegisterStartupScript(this, GetType(), "CreateError", $"alert('Error al ejecutar el query: {ex.Message}');", true);

                CargarListaTablas();
            }
        }

        private bool TablaExiste(string nombreTabla, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{nombreTabla}'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool EsAlfanumerico(string input)
        {
            return !string.IsNullOrEmpty(input) && input.All(char.IsLetterOrDigit) && !input.All(char.IsDigit);
        }

        private bool EsFormatoCorrecto(string camposTexto)
        {
            return !string.IsNullOrWhiteSpace(camposTexto) && camposTexto.Contains(",");
        }

        protected void verificarButton_Click(object sender, EventArgs e)
        {
            string query = sqlQueryTextBox.Text;
            string resultado = VeriQue(query);
            resultadoLabel.InnerText = resultado;

            // Script para mostrar el mensaje en un alert
            string script = "alert('" + resultado + "');";
            ScriptManager.RegisterStartupScript(this, GetType(), "QueryVerificationScript", script, true);
        }

        public string VeriQue(string query)
        {
            string connectionString = "Data Source=LAPTOP-G02OT5LT;Initial Catalog=Create tables;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        return "El Query SQL se ejecutó correctamente.";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error al ejecutar el Query SQL: " + ex.Message;
            }
        }
        protected void btnGetConnection_ServerClick(object sender, EventArgs e)
        {
            string database = txtName.Value;
            string server = txtServer.Value;
            if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(server))
            {
                // Si uno de los campos está vacío, mostrar un mensaje de error
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Por favor, ingresa el nombre de la base de datos y el servidor.')", true);
                return; // Salir del método
            }
            try
            {
                // Construir la cadena de conexión utilizando los valores obtenidos del formulario
                string connectionString = $"Data Source={server};Initial Catalog={database};Integrated Security=True";

                // Intentar abrir la conexión
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Si la conexión se abre correctamente, significa que la base de datos y el servidor existen
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('¡La conexión fue exitosa!')", true);
                }
            }
            catch (Exception ex)
            {
                // Si ocurre alguna excepción, significa que la base de datos o el servidor no existen o hay un error de conexión
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('¡Tu nombre de servidor o tu nombre de base de datos están mal escritos o no existen! Por favor, verifica e intenta de nuevo.')", true);
            }
        }
    }
}

