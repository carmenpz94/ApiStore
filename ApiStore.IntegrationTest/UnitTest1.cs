using ApiStore.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace ApiStore.IntegrationTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            // Crear instancia de la aplicación en memoria
            using var application = new WebApplicationFactory<Program>();

            // Crear cliente HTTP para enviar solicitudes
            using var _httpClient = application.CreateClient();

            var userSession = new UserRequest { Username = "Ariel", Userpassword = "12345" };
            var response = await _httpClient.PostAsJsonAsync("api/users/login", userSession);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<string>();
            }

        }
    }
}