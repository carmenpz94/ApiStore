using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ApiStore.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiStore.IntegrationTest
{
    [TestClass]
    public class UserEndpointsTest
    {
        private static HttpClient _httpClient;
        private static WebApplicationFactory<Program> _factory;
        private static string _token;

        /// <summary>
        /// Configurar entorno de prueba inicializando la API y obteniendo el token JWT
        /// </summary>

        [ClassInitialize]
        public static async Task ClassInit(TestContext context)
        {
            // Crear instancia de la aplicación en memoria
            _factory = new WebApplicationFactory<Program>();

            // Crear el cliente HTTP
            _httpClient = _factory.CreateClient();

            // Arrange: Preparar la carga útil para el inicio de sesión
            var loginRequest = new UserRequest
            {
                Username = "usuario1",
                Userpassword = "contraseña123",
            };

            // Act: Enviar la solicitud de inicio de sesión
            var loginResponse = await _httpClient.PostAsJsonAsync("api/users/login", loginRequest);

            // Assert: Verificar que el inicio de sesión sea exitoso
            loginResponse.EnsureSuccessStatusCode();
            _token = (await loginResponse.Content.ReadAsStringAsync()).Trim('"');
        }

        [TestInitialize]
        public void AgregarTokenAlaCabecera()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _token
            );
        }

        [TestMethod]
        public async Task ObtenerUsuarios_ConTokenValido_RetornaListaDeUsuarios()
        {
            // Arrange: Pasar autorización a la cabecera
            AgregarTokenAlaCabecera();

            // Act: Realizar solicitud para obtener los detalles de ordenes
            var orderDetails = await _httpClient.GetFromJsonAsync<List<UserResponse>>("api/users");

            // Assert: Verificar que la lista de detalle de ordenes no sea nula y que tenga elementos
            Assert.IsNotNull(orderDetails, "La lista de USUARIOS no debería ser nula.");
            Assert.IsTrue(
                orderDetails.Count > 0,
                "La lista de usuarios debería contener al menos un elemento."
            );
        }

        [TestMethod]
        public async Task ObtenerUsuariosPorId_UsuariosExistente_RetornarUsuarios()
        {
            // Arrange: Pasar autorización a la cabecera y establecer ID de detalle de orden existente
            AgregarTokenAlaCabecera();
            var UserId = 1;

            // Act: Realizar solicitud para obtener detalle de ordenes por ID
            var user = await _httpClient.GetFromJsonAsync<UserResponse>($"api/users/{UserId}");

            // Assert: Verificar que el detalle de orden no sea nulo y que tenga el ID correcto
            Assert.IsNotNull(user, "El USUARIO no debería ser nulo.");
            Assert.AreEqual(UserId, user.Id, "El ID del usuarios devuelto no coincide.");
        }

        [TestMethod]
        public async Task GuardarUsuarios_ConDatosValidos_RetornarCreated()
        {
            // Arrange: Pasar autorización a la cabecera y preparar el nuevo usuario
            AgregarTokenAlaCabecera();

            var newUser = new UserRequest
            {
                Username = "ChrisVilla2",
                Userpassword = "ChrisVilla2",
                UserRole = "Administrador2",
            };

            // Act: Realizar solicitud para guardar nuevo usuario
            var response = await _httpClient.PostAsJsonAsync("api/users", newUser);

            // Assert: Verificar el código y estado será Created
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task GuardarUsuario_UsernameDuplicado_RetornaConflict()
        {
            // Arrange: Pasar autorización a la cabecera y preparar el usuario duplicado
            AgregarTokenAlaCabecera();

            var newUser = new UserRequest
            {
                Username = "ChrisVilla2",
                Userpassword = "ChrisVilla2",
                UserRole = "Administrador2",
            };

            // Act: Realizar solicitud para guardar el usuario con nombre de usuario duplicado
            var response = await _httpClient.PostAsJsonAsync("api/users", newUser);

            // Assert: Verifica el código de estado Conflict
            Assert.AreEqual(
                HttpStatusCode.Conflict,
                response.StatusCode,
                "Se esperaba un conflicto al intentar crear usuario duplicado"
            );
        }

        [TestMethod]
        public async Task ModificarUsuario_UsuarioExistente_Retornar()
        {
            AgregarTokenAlaCabecera();

            //ARRANGE: PASAR AUTORIZACIÓN A LA CABECERA Y PREPARAR EL USUARIO MODIFICADO
            var existingUser = new UserRequest
            {
                Username = "usuario1",
                Userpassword = "contraseña123",
                UserRole = "Cliente",
            };
            var userId = 1;

            var response = await _httpClient.PutAsJsonAsync($"api/users/{userId}", existingUser);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode,
                "EL USUARIO SE MODIFICO CORRECTAMENTE"
            );
        }

        [TestMethod]
        public async Task EliminarUsuario_UserExistente_RetornaNoContent()
        {
            // Arrange: Pasar autorización a la cabecera, pasando un ID
            AgregarTokenAlaCabecera();
            var userId = 45;

            var response = await _httpClient.DeleteAsync($"api/users/{userId}");

            // Assert: Verifica que la respuesta se NoContent
            Assert.AreEqual(
                HttpStatusCode.NoContent,
                response.StatusCode,
                "El detalle de ordenes no se eliminó correctamente"
            );
        }

        [TestMethod]
        public async Task EliminarUsuario_UsuarioNoExistente_RetornaNotFound()
        {
            // Arrange: Pasar autorización a la cabecera, pasando un ID
            AgregarTokenAlaCabecera();
            var UserId = 3;

            // Act: Realizar solicitud para eliminar detalle de ordenes existente
            var response = await _httpClient.DeleteAsync($"api/users/{UserId}");

            // Assert: Verifica que la respuesta es NotFound
            Assert.AreEqual(
                HttpStatusCode.NotFound,
                response.StatusCode,
                "Se esperaba un 404 NotFound al intentar eliminar un usuario existente inexistente."
            );
        }
    }
}
