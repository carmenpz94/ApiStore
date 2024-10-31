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
using ApiStore.Models;


namespace ApiStore.IntegrationTest
{

    [TestClass]
    public class ProductEndpointsTests
    {
        private static HttpClient _httpClient;
        private static WebApplicationFactory<Program> _factory;
        private static string _token;
        private string cartera;
        private string cafe;

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
            var loginRequest = new UserRequest { Username = "carmen", Userpassword = "carmen" };

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
        public async Task ObteneProducts_ConTokenValido_RetornaListaDeProducts()
        {
            // Arrange: Pasar autorización a la cabecera
            AgregarTokenAlaCabecera();

            // Act: Realizar solicitud para obtener los productos
            var products = await _httpClient.GetFromJsonAsync<List<ProductResponse>>("/api/products");

            // Assert: Verificar que la lista de producto no sea nula y que tenga elementos
            Assert.IsNotNull(products, "La lista de Productos no debería ser nula.");
            Assert.IsNotNull(products, "La lista de Productos no debería ser nula.");
            Assert.IsTrue(products.Count > 0, "La lista de Productos debería contener al menos un elemento.");
        }

        [TestMethod]
        public async Task ObtenerProductPorId_ProductExistente_Product()
        {
            // Arrange: Pasar autorización a la cabecera y establecer ID de producto existente
            AgregarTokenAlaCabecera();
            var id = 14;

            // Act: Realizar solicitud para obtener producto por ID
            var products = await _httpClient.GetFromJsonAsync<ProductResponse>($"/api/products/{id}");

            // Assert: Verificar que el producto no sea nulo y que tenga el ID correcto
            Assert.IsNotNull(products, "El Producto no debería ser nulo.");
            Assert.AreEqual(id, products?.Id, "El ID del Product devuelto no coincide.");
        }

        [TestMethod]
        public async Task GuardarProducts_ConDatosValidos_RetornaCreated()
        {
            // Arrange: Pasar autorización a la cabecera y preparar el nuevo producto
            AgregarTokenAlaCabecera();
            var newproducts = new ProductRequest { Nombre = "cartera", Descripcion = "cafe", Precio = 5, CategoriaId = 4 ,Stock =11, Imagen = "1102png" };
            // Act: Realizar solicitud para guardar el producto
            var response = await _httpClient.PostAsJsonAsync("api/products", newproducts);
            // Assert: Verifica el código de estado Created
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "El Producto no se creó correctamente");
        }

        [TestMethod]
        public async Task ModificarProducts_ProductsExistente_RetornaOk()
        {
            // Arrange: Pasar autorización a la cabecera y preparar el producto modificado, pasando un ID
            AgregarTokenAlaCabecera();
            var existingproducts = new ProductRequest { Nombre = "zapatos", Descripcion = "negros"  , Precio = 5, CategoriaId = 4, Stock = 11, Imagen = "1102png" };
            var id = 17;

            // Act: Realizar solicitud para modificar un producto existente
            var response = await _httpClient.PutAsJsonAsync($"/api/products/{id}", existingproducts);

            // Assert: Verifica que la respuesta se OK
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "El producto no se modificó correctamente");
        }


        [TestMethod]
        public async Task EliminarProducts_ProductsExistente_RetornaNoContent()
        {
            // Arrange: Pasar autorización a la cabecera, pasando un ID
            AgregarTokenAlaCabecera();
            var id = 6;

            // Act: Realizar solicitud para eliminar un producto existente
            var response = await _httpClient.DeleteAsync($"/api/products/{id}");

            // Assert: Verifica que la respuesta se NoContent
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode, "El producto no se eliminó correctamente");
        }

        [TestMethod]
        public async Task EliminarProducts_ProductsNoExistente_RetornaNotFound()
        {
            // Arrange: Pasar autorización a la cabecera, pasando un ID
            AgregarTokenAlaCabecera();
            var id = 2;

            // Act: Realizar solicitud para eliminar producto existente
            var response = await _httpClient.DeleteAsync($"/api/products/{id}");

            // Assert: Verifica que la respuesta es NotFound
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode,
                "Se esperaba un 404 NotFound al intentar eliminar un Producto existente inexistente.");
        }




    }
}
