<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Prueba_final.Views.main" %>

<!DOCTYPE html>
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-image: url('https://content.elmueble.com/medio/2024/03/24/plantas-en-estantes_00d66678_240324185349_600x600.jpg');
            background-size: cover;
            background-repeat: no-repeat;
        }

        #form1 {
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            background-color: rgba(255, 255, 255, 0.8); /* Añade un fondo semi-transparente para mejorar la legibilidad del contenido */
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        h1 {
            text-align: center;
            margin-bottom: 20px;
        }

        #ListBoxTables {
            width: 100%;
            margin-bottom: 10px;
        }

        .card {
            border: 1px solid #ccc;
            border-radius: 5px;
            padding: 10px;
            margin: 10px;
            width: 300px;
            float: left;
        }

        .container {
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            background-color: rgba(255, 255, 255, 0.8); /* Añade un fondo semi-transparente para mejorar la legibilidad del contenido */
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        .result {
            margin-top: 20px;
            text-align: center;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Prueba Final</h1>
        <div class="container">
            <h2 class="mb-4">Crear Tabla</h2>
            <div class="mb-3">
                <label for="nombreTabla" class="form-label">Nombre de la Tabla:</label>
                <asp:TextBox ID="nombreTabla" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="mb-3">
                <label for="campos" class="form-label">Nombres de Campos (separados por coma):</label>
                <asp:TextBox ID="campos" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div>
                <asp:Button ID="crearTablaBtn" runat="server" Text="Crear Tabla" OnClick="crearTablaBtn_Click" CssClass="btn btn-primary" />
            </div>
        </div>

        <div>
            <h2>Mis Tablas</h2>
            <h4>Debes primero crear tablas para poder ver tus tablas</h4>
            <asp:ListBox ID="ListBoxTables" runat="server" Rows="5"></asp:ListBox>
            <br />
            <asp:Button ID="ButtonDelete" runat="server" Text="Eliminar Tabla Seleccionada" OnClick="ButtonDelete_Click" CssClass="btn btn-danger" />
        </div>
        <div class="container">
            <h2>Verificador de Query SQL</h2>
            <label for="sqlQuery">Digite el Query a verificar</label><br />
            <asp:TextBox ID="sqlQueryTextBox" runat="server" TextMode="MultiLine" Rows="4" Columns="50"></asp:TextBox><br />
            <asp:Button ID="verificarButton" runat="server" Text="Verificar Query" OnClick="verificarButton_Click" CssClass="btn btn-primary" />
            <div class="result" id="resultadoLabel" runat="server"></div>
        </div>

        <div class="container">
            <h2>Verificador Cadena de Conexion</h2>
            <label for="txtServer">Nombre del Servidor:</label>
            <input type="text" id="txtServer" runat="server" class="form-control" />
            <br />
            <label for="txtName">Nombre de la Base de Datos:</label>
            <input type="text" id="txtName" runat="server" class="form-control" />
            <br />
            <asp:Button ID="btnGetConnection" runat="server" Text="Obtener Conexión" OnClick="btnGetConnection_ServerClick" CssClass="btn btn-primary" />
        </div>
    </form>
    <!-- Bootstrap JS -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz" crossorigin="anonymous"></script>
</body>
</html>


