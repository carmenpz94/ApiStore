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
    public class OrderEndpointsTests
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
            _factory = new WebApplicationFactory<Program>();
            _httpClient = _factory.CreateClient();

            var loginRequest = new UserRequest { Username = "Ariel", Userpassword = "12345" };
            var loginResponse = await _httpClient.PostAsJsonAsync("api/users/login", loginRequest);

            loginResponse.EnsureSuccessStatusCode();
            _token = (await loginResponse.Content.ReadAsStringAsync()).Trim('"');
        }

        [TestInitialize]
        public void AgregarTokenAlaCabecera()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        [TestMethod]
        public async Task ObtenerOrders_ConTokenValido_RetornaListaDeOrders()
        {
            AgregarTokenAlaCabecera();
            var orders = await _httpClient.GetFromJsonAsync<List<OrderResponse>>("/api/orders");

            Assert.IsNotNull(orders, "La lista de órdenes no debería ser nula.");
            Assert.IsTrue(orders.Count > 0, "La lista de órdenes debería contener al menos un elemento.");
        }

        [TestMethod]
        public async Task ObtenerOrderPorId_OrderExistente_Order()
        {
            AgregarTokenAlaCabecera();
            var id = 2;

            var order = await _httpClient.GetFromJsonAsync<OrderResponse>($"/api/orders/{id}");

            Assert.IsNotNull(order, "La orden no debería ser nula.");
            Assert.AreEqual(id, order?.Id, "El ID de la orden devuelta no coincide.");
        }

        [TestMethod]
        public async Task GuardarOrder_ConDatosValidos_RetornaCreated()
        {
            AgregarTokenAlaCabecera();
            var newOrder = new OrderRequest { UserId = 2, EstadoPedido = "Pendiente" };

            var response = await _httpClient.PostAsJsonAsync("api/orders", newOrder);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "La orden no se creó correctamente");
        }

        [TestMethod]
        public async Task ModificarOrder_OrderExistente_RetornaOk()
        {
            AgregarTokenAlaCabecera();
            var existingOrder = new OrderRequest { ClienteId = 1, EstadoPedido = "Enviado" };
            var id = 2;

            var response = await _httpClient.PutAsJsonAsync($"/api/orders/{id}", existingOrder);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "La orden no se modificó correctamente");
        }

        [TestMethod]
        public async Task EliminarOrder_OrderExistente_RetornaNoContent()
        {
            AgregarTokenAlaCabecera();
            var id = 2;

            var response = await _httpClient.DeleteAsync($"/api/orders/{id}");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "La orden no se eliminó correctamente");
        }

        [TestMethod]
        public async Task EliminarOrder_OrderNoExistente_RetornaNotFound()
        {
            AgregarTokenAlaCabecera();
            var id = 99;

            var response = await _httpClient.DeleteAsync($"/api/orders/{id}");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Se esperaba un 404 NotFound al intentar eliminar una orden inexistente.");
        }
    }
}
