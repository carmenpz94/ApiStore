using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using ApiStore.DTOs;
using System.Net.Http.Json;
using System.Net;

namespace ApiStore.IntegrationTest
{
    [TestClass]
    public class OrderDetailEndpointsTests
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
            var loginRequest = new UserRequest { Username = "erickz", Userpassword = "1234" };

            // Act: Enviar la solicitud de inicio de sesión
            var loginResponse = await _httpClient.PostAsJsonAsync("api/users/login", loginRequest);

            // Assert: Verificar que el inicio de sesión sea exitoso
            loginResponse.EnsureSuccessStatusCode();
            _token = (await loginResponse.Content.ReadAsStringAsync()).Trim('"');
        }

            [TestInitialize]

            public void AgregarTokenAlaCabecera()
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }

        [TestMethod]
        public async Task ObtenerOrderDetails_ConTokenValido_RetornaListaDeOrderDetails()
        {
            // Arrange: Pasar autorización a la cabecera
            AgregarTokenAlaCabecera();

            // Act: Realizar solicitud para obtener los detalles de ordenes
            var orderDetails = await _httpClient.GetFromJsonAsync<List<OrderDetailResponse>>("/api/orderDetails");

            // Assert: Verificar que la lista de detalle de ordenes no sea nula y que tenga elementos
            Assert.IsNotNull(orderDetails, "La lista de detalle de ordenes no debería ser nula.");
            Assert.IsTrue(orderDetails.Count > 0, "La lista de detalle de ordenes debería contener al menos un elemento.");
        }

        [TestMethod]
        public async Task ObtenerOrderDetailsPorId_OrderDetailsExistente_OrderDetails()
        {
            // Arrange: Pasar autorización a la cabecera y establecer ID de detalle de orden existente
            AgregarTokenAlaCabecera();
            var id = 4;

            // Act: Realizar solicitud para obtener detalle de ordenes por ID
            var orderDetail = await _httpClient.GetFromJsonAsync<OrderDetailResponse>($"/api/orderDetails/{id}");

            // Assert: Verificar que el detalle de orden no sea nulo y que tenga el ID correcto
            Assert.IsNotNull(orderDetail, "El detalle de órdenes no debería ser nulo.");
            Assert.AreEqual(id, orderDetail?.Id, "El ID del detalle de órdenes devuelto no coincide.");
        }

        [TestMethod]
        public async Task GuardarOrderDerails_ConDatosValidos_RetornaCreated()
        {
            // Arrange: Pasar autorización a la cabecera y preparar el nuevo detalle de ordenes
            AgregarTokenAlaCabecera();
            var newOrderDetail = new OrderDetailRequest { Cantidad = 2, Precio = 5, OrderId = 1, ProductId = 2};
            // Act: Realizar solicitud para guardar el detalle de ordenes
            var response = await _httpClient.PostAsJsonAsync("api/orderDetails", newOrderDetail);
            // Assert: Verifica el código de estado Created
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "El detalle de ordenes no se creó correctamente");
        }

        [TestMethod]
        public async Task ModificarOrderDetail_OrderDetailExistente_RetornaOk()
        {
            // Arrange: Pasar autorización a la cabecera y preparar el detalle de ordenes modificado, pasando un ID
            AgregarTokenAlaCabecera();
            var existingOrderDetail = new OrderDetailRequest { Cantidad = 5, Precio = 12, OrderId = 1, ProductId = 2 };
            var id = 5;

            // Act: Realizar solicitud para modificar detalle de ordenes existente
            var response = await _httpClient.PutAsJsonAsync($"/api/orderDetails/{id}", existingOrderDetail);

            // Assert: Verifica que la respuesta se OK
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "El detalle de ordenes no se modificó correctamente");
        }


        [TestMethod]
        public async Task EliminarOrderDetail_OrderDetailExistente_RetornaNoContent()
        {
            // Arrange: Pasar autorización a la cabecera, pasando un ID
            AgregarTokenAlaCabecera();
            var id = 8;

            // Act: Realizar solicitud para eliminar detalle de ordenes existente
            var response = await _httpClient.DeleteAsync($"/api/orderDetails/{id}");

            // Assert: Verifica que la respuesta se NoContent
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "El detalle de ordenes no se eliminó correctamente");
        }

        [TestMethod]
        public async Task EliminarOrderDetail_OrderDetailNoExistente_RetornaNotFound()
        {
            // Arrange: Pasar autorización a la cabecera, pasando un ID
            AgregarTokenAlaCabecera();
            var id = 2;

            // Act: Realizar solicitud para eliminar detalle de ordenes existente
            var response = await _httpClient.DeleteAsync($"/api/orderDetails/{id}");
            
            // Assert: Verifica que la respuesta es NotFound
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode,
                "Se esperaba un 404 NotFound al intentar eliminar un detalle de ordene existente inexistente.");
        }







    }


}

