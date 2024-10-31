using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiStore.DTOs;
using ApiStore.Services.Users;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiStore.Endponits
{
    public static class UserEndpoints
    {
        public static void Add(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/users").WithTags("Usuarios");

            // GET all Users
            group
                .MapGet(
                    "/",
                    async (IUserServices userServices) =>
                    {
                        var users = await userServices.GetUsers();
                        return Results.Ok(users);
                    }
                )
                .WithOpenApi(o => new OpenApiOperation(o)
                {
                    Summary = "OBTENER USUARIOS",
                    Description = "MUESTRA UNA LISTA DE TODOS LOS USUARIOS",
                }).RequireAuthorization();

            // GET user by ID
            group
                .MapGet(
                    "/{id}",
                    async (int id, IUserServices userServices) =>
                    {
                        var user = await userServices.GetUser(id);
                        return user is not null ? Results.Ok(user) : Results.NotFound();
                    }
                )
                .WithOpenApi(o => new OpenApiOperation(o)
                {
                    Summary = "OBTENER UN USUARIO POR ID",
                    Description = "OBTIENE UN USUARIO DADO SU ID",
                }).RequireAuthorization();

            // POST create a new user
            group
                .MapPost(
                    "/",
                    async (UserRequest user, IUserServices userServices) =>
                    {
                        try
                        {
                            var createdUserId = await userServices.PostUser(user);
                            return Results.Created($"api/users/{createdUserId}", user);
                        }
                        catch (Exception)
                        {
                            return Results.Conflict("EL NOMBRE DE USUARIO YA ESTÁ EN USO.");
                        }
                    }
                )
                .WithOpenApi(o => new OpenApiOperation(o)
                {
                    Summary = "CREAR NUEVO USUARIO",
                    Description = "CREA UN NUEVO USUARIO CON LOS DATOS PROPORCIONADOS",
                });

            // PUT modified a new User
            group
                .MapPut(
                    "/{id}",
                    async (int id, UserRequest user, IUserServices userServices) =>
                    {
                        var isUpdated = await userServices.PutUser(id, user);

                        if (isUpdated == -1)
                            return Results.NotFound();
                        else
                            return Results.Ok(user);
                    }
                )
                .WithOpenApi(o => new OpenApiOperation(o)
                {
                    Summary = "MODIFICAR UN USUARIO",
                    Description = "ACTUALIZA UN USUARIO DADO SU ID",
                }).RequireAuthorization();

            // DELETE product by ID
            group
                .MapDelete(
                    "/{id}",
                    async (int id, IUserServices userServices) =>
                    {
                        var result = await userServices.DeleteUser(id);
                        if (result == -1)
                            return Results.NotFound();
                        else
                            return Results.NoContent();
                    }
                )
                .WithOpenApi(o => new OpenApiOperation(o)
                {
                    Summary = "ELIMINAR UN USUARIO",
                    Description = "ELIMINAR UN USUARIO DADO SU ID",
                }).RequireAuthorization();

            group
                .MapPost(
                    "/login",
                    async (UserRequest user, IUserServices userServices, IConfiguration config) =>
                    {
                        var login = await userServices.Login(user);

                        if (login is null)
                        {
                            return Results.Unauthorized();
                        }
                        else
                        {
                            var jwtSettings = config.GetSection("JwtSetting");
                            var secretKey = jwtSettings.GetValue<String>("SecretKey");
                            var issuer = jwtSettings.GetValue<String>("Issuer");
                            var audience = jwtSettings.GetValue<String>("Audience");

                            var tokenHandler = new JwtSecurityTokenHandler();
                            var key = Encoding.UTF8.GetBytes(secretKey);

                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = new ClaimsIdentity(
                                    new[]
                                    {
                                        new Claim(ClaimTypes.Name, login.Username),
                                        new Claim(ClaimTypes.Role, login.UserRole),
                                    }
                                ),
                                Expires = DateTime.UtcNow.AddHours(1),
                                Issuer = issuer,
                                Audience = audience,
                                SigningCredentials = new SigningCredentials(
                                    new SymmetricSecurityKey(key),
                                    SecurityAlgorithms.HmacSha256Signature
                                ),
                            };

                            //CREAR TOKEN
                            var token = tokenHandler.CreateToken(tokenDescriptor);

                            var jwt = tokenHandler.WriteToken(token);

                            return Results.Ok(jwt);
                        }
                    }
                )
                .WithOpenApi(o => new OpenApiOperation(o)
                {
                    Summary = "LOGIN USUARIO",
                    Description = "GENERARÁ EL TOKEN PARA INICIO DE SESIÓN.",
                });
        }
    }
}
