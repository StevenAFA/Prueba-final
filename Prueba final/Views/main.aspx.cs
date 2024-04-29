using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;


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

        protected void VerificarQuery_Click(object sender, EventArgs e)
        {
            string sqlQuery = sqlQueryTextBox.Text.Trim().ToUpper();

            string alertScript = "";

            if (sqlQuery.StartsWith("CREATE TABLE"))
            {
                if (EsQueryCreateTableValido(sqlQuery))
                {
                    alertScript = "alert('El Query es una sentencia CREATE TABLE y está bien escrita.');";
                }
                else
                {
                    alertScript = "alert('El Query CREATE TABLE no es válido. Verifique la sintaxis y asegúrese de que esté completo.');";
                }
            }
            else if (sqlQuery.StartsWith("UPDATE"))
            {
                if (EsQueryUpdateValido(sqlQuery))
                {
                    alertScript = "alert('El Query es una sentencia UPDATE y está bien escrita.');";
                }
                else
                {
                    alertScript = "alert('El Query UPDATE no es válido. Verifique la sintaxis y asegúrese de que esté completo.');";
                }
            }
            else if (sqlQuery.StartsWith("INSERT INTO"))
            {
                if (EsQueryInsertValido(sqlQuery))
                {
                    alertScript = "alert('El Query es una sentencia INSERT INTO y está bien escrita.');";
                }
                else
                {
                    alertScript = "alert('El Query INSERT INTO no es válido. Verifique la sintaxis y asegúrese de que esté completo.');";
                }
            }
            else if (sqlQuery.StartsWith("DROP TABLE"))
            {
                if (EsQueryDropTableValido(sqlQuery))
                {
                    alertScript = "alert('El Query es una sentencia DROP TABLE y está bien escrita.');";
                }
                else
                {
                    alertScript = "alert('El Query DROP TABLE no es válido. Verifique la sintaxis y asegúrese de que esté completo.');";
                }
            }
            else
            {
                alertScript = "alert('El Query no corresponde a ninguna sentencia SQL permitida o está mal escrito.');";
            }

            // Mostrar alerta
            ScriptManager.RegisterStartupScript(this, GetType(), "QueryVerification", alertScript, true);
        }

        private bool EsQueryCreateTableValido(string sqlQuery)
        {
            // Verificar que el query contenga la frase "CREATE TABLE"
            int createTableIndex = sqlQuery.IndexOf("CREATE TABLE", StringComparison.OrdinalIgnoreCase);
            if (createTableIndex == -1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "CreateTableInvalid", "alert('El Query no contiene la frase \"CREATE TABLE\".');", true);
                return false;
            }

            // Obtener la parte del query que sigue a "CREATE TABLE"
            string remainingQuery = sqlQuery.Substring(createTableIndex + "CREATE TABLE".Length).Trim();

            // Verificar si hay un paréntesis de apertura después de "CREATE TABLE"
            int openParenIndex = remainingQuery.IndexOf('(');
            if (openParenIndex == -1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "CreateTableInvalid", "alert('El Query CREATE TABLE no contiene un paréntesis de apertura después de la frase \"CREATE TABLE\".');", true);
                return false;
            }

            // Obtener el nombre de la tabla
            string tableName = remainingQuery.Substring(0, openParenIndex).Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "CreateTableInvalid", "alert('El nombre de la tabla está vacío después de la frase \"CREATE TABLE\".');", true);
                return false;
            }

            // Verificar si hay un paréntesis de cierre después del nombre de la tabla
            int closeParenIndex = remainingQuery.IndexOf(')', openParenIndex);
            if (closeParenIndex == -1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "CreateTableInvalid", "alert('El Query CREATE TABLE no contiene un paréntesis de cierre después del nombre de la tabla.');", true);
                return false;
            }

            // Obtener la parte del query que contiene las columnas
            string columnsPart = remainingQuery.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1).Trim();

            // Verificar si las columnas están vacías
            if (string.IsNullOrEmpty(columnsPart))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "CreateTableInvalid", "alert('El Query CREATE TABLE no contiene ninguna columna después del nombre de la tabla.');", true);
                return false;
            }

            return true;
        }


        private bool EsQueryUpdateValido(string sqlQuery)
        {
            // Verificar que el query comience con "UPDATE"
            if (!sqlQuery.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateQueryInvalid", "alert('El Query no comienza con la frase \"UPDATE\".');", true);
                return false;
            }

            // Utilizar expresión regular para buscar la cláusula SET
            Regex setRegex = new Regex(@"\bSET\b", RegexOptions.IgnoreCase);
            if (!setRegex.IsMatch(sqlQuery))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateQueryInvalid", "alert('El Query UPDATE no contiene la frase \"SET\" después del nombre de la tabla.');", true);
                return false;
            }

            // Utilizar expresión regular para buscar la cláusula WHERE
            Regex whereRegex = new Regex(@"\bWHERE\b", RegexOptions.IgnoreCase);
            if (!whereRegex.IsMatch(sqlQuery))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "UpdateQueryInvalid", "alert('El Query UPDATE no contiene la cláusula \"WHERE\".');", true);
                return false;
            }

            return true;
        }

        private bool EsQueryInsertValido(string sqlQuery)
        {
            // Verificar que el query comience con "INSERT INTO"
            if (!sqlQuery.StartsWith("INSERT INTO", StringComparison.OrdinalIgnoreCase))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "InsertQueryInvalid", "alert('El Query no comienza con la frase \"INSERT INTO\".');", true);
                return false;
            }

            // Obtener la parte del query que sigue a "INSERT INTO"
            int insertIndex = sqlQuery.IndexOf("INSERT INTO", StringComparison.OrdinalIgnoreCase) + "INSERT INTO".Length;
            string remainingQuery = sqlQuery.Substring(insertIndex).Trim();

            // Buscar el índice de la lista de columnas (entre paréntesis)
            int openParenIndex = remainingQuery.IndexOf('(');
            int closeParenIndex = remainingQuery.IndexOf(')');
            if (openParenIndex == -1 || closeParenIndex == -1 || closeParenIndex <= openParenIndex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "InsertQueryInvalid", "alert('El Query INSERT INTO no contiene una lista de columnas válida.');", true);
                return false;
            }
            // Verificar que el siguiente token después de la lista de columnas sea la palabra clave "VALUES"
            int valuesIndex = remainingQuery.IndexOf("VALUES", StringComparison.OrdinalIgnoreCase);
            if (valuesIndex == -1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "InsertQueryInvalid", "alert('El Query INSERT INTO no contiene la palabra clave \"VALUES\" después de la lista de columnas.');", true);
                return false;
            }

            // Verificar que hay datos después de la palabra clave "VALUES"
            string valuesPart = remainingQuery.Substring(valuesIndex + "VALUES".Length).Trim();
            if (string.IsNullOrEmpty(valuesPart))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "InsertQueryInvalid", "alert('El Query INSERT INTO no contiene valores después de la palabra clave \"VALUES\".');", true);
                return false;
            }

            return true;
        }

        private bool EsQueryDropTableValido(string sqlQuery)
        {
            // Verificar que el query comience con "DROP TABLE"
            if (!sqlQuery.StartsWith("DROP TABLE", StringComparison.OrdinalIgnoreCase))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DropTableQueryInvalid", "alert('El Query no comienza con la frase \"DROP TABLE\".');", true);
                return false;
            }

            // Obtener la parte del query que sigue a "DROP TABLE"
            int dropIndex = sqlQuery.IndexOf("DROP TABLE", StringComparison.OrdinalIgnoreCase) + "DROP TABLE".Length;
            string remainingQuery = sqlQuery.Substring(dropIndex).Trim();

            // Verificar si hay un nombre de tabla después de "DROP TABLE"
            if (string.IsNullOrWhiteSpace(remainingQuery))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DropTableQueryInvalid", "alert('El Query DROP TABLE no especifica un nombre de tabla válido.');", true);
                return false;
            }

            // Verificar si el nombre de la tabla contiene caracteres válidos
            if (remainingQuery.IndexOfAny(new char[] { ' ', ';' }) == -1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DropTableQueryInvalid", "alert('El Query DROP TABLE no especifica un nombre de tabla válido.');", true);
                return false;
            }

            // Obtener el nombre de la tabla
            int spaceIndex = remainingQuery.IndexOf(' ');
            string tableName = spaceIndex != -1 ? remainingQuery.Substring(0, spaceIndex).Trim() : remainingQuery;

            // Verificar si hay datos después del nombre de la tabla
            if (string.IsNullOrWhiteSpace(tableName))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "DropTableQueryInvalid", "alert('El Query DROP TABLE no especifica un nombre de tabla válido.');", true);
                return false;
            }

            return true;
        }
        protected void btnTestConnection_Click(object sender, EventArgs e)
        {
            string connectionString = txtConnectionString.Text;

            if (EsFormatoConexionSql(connectionString) || EsFormatoConexionWindowsAuth(connectionString) || EsFormatoConexionDesdeConfig(connectionString))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ConnectionSuccess", "alert('La cadena de conexión es válida.');", true);      
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ConnectionError", "alert('La cadena de conexión no es válida.');", true);
            }
        }

        private bool EsFormatoConexionSql(string connectionString)
        {
            Regex regex = new Regex(@"Data Source=.+;Initial Catalog=.+;User ID=.+;Password=.+;");
            if (regex.IsMatch(connectionString))
            {
                return true;
            }
            else
            {
                // Mostrar mensaje de error detallado
                ScriptManager.RegisterStartupScript(this, GetType(), "ConnectionError", "alert('La cadena de conexión SQL no es válida. Verifique que contenga Data Source, Initial Catalog, User ID y Password.');", true);
                return false;
            }
        }

        private bool EsFormatoConexionWindowsAuth(string connectionString)
        {
            Regex regex = new Regex(@"Data Source=.+;Initial Catalog=.+;Integrated Security=True;");
            if (regex.IsMatch(connectionString))
            {
                return true;
            }
            else
            {
                // Mostrar mensaje de error detallado
                ScriptManager.RegisterStartupScript(this, GetType(), "ConnectionError", "alert('La cadena de conexión con autenticación de Windows no es válida. Verifique que contenga Data Source, Initial Catalog e Integrated Security=True.');", true);
                return false;
            }
        }

        private bool EsFormatoConexionDesdeConfig(string connectionString)
        {
            Regex regex = new Regex(@"<connectionStrings>\s*<add name=.+connectionString=.+providerName=.+/>\s*</connectionStrings>");
            if (regex.IsMatch(connectionString))
            {
                return true;
            }
            else
            {
                // Mostrar mensaje de error detallado
                ScriptManager.RegisterStartupScript(this, GetType(), "ConnectionError", "alert('La cadena de conexión desde la configuración no es válida. Verifique el formato y los atributos name, connectionString y providerName.');", true);
                return false;
            }
        }
    }
}

