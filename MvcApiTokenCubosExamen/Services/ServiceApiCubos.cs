using MvcApiTokenCubosExamen.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;

namespace MvcApiTokenCubosExamen.Services
{
    public class ServiceApiCubos
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiCubos;

        public ServiceApiCubos(IConfiguration configuration)
        {
            this.UrlApiCubos =
                 configuration.GetValue<string>("ApiUrls:UrlApiCubos");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<string> GetTokenAsync
         (string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                    await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                    jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                        await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response =
                        await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            string request = "/api/cubos";
            List<Cubo> cubos =
                await this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }

        public async Task<List<Cubo>> FindCuboAsync(string marca)
        {
            string request = "/api/cubos/" + marca;
            List<Cubo> cubos =
                await this.CallApiAsync<List<Cubo>>(request);
            return cubos;
        }

        public async Task NewUsuarioAsync(string nombre, string email, string pass, string imagen)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/cubos/newusuario";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Usuario usuario = new Usuario
                {
                    Nombre = nombre,
                    Email = email,
                    Pass = pass,
                    Imagen = imagen
                };
                string jsonCubo =
                    JsonConvert.SerializeObject(usuario);
                StringContent content =
                    new StringContent(jsonCubo, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);

            }
        }

        public async Task NewCuboAsync(string nombre, string marca, string imagen, int precio)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/cubos";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Cubo cubo = new Cubo
                {
                    Nombre = nombre,
                    Marca = marca,
                    Imagen = imagen,
                    Precio = precio
                };
                string jsonCubo =
                    JsonConvert.SerializeObject(cubo);
                StringContent content =
                    new StringContent(jsonCubo, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);

            }
        }

        public async Task<Usuario> GetPerfilUsuarioAsync
                (string token)
        {
            string request = "/api/cubos/perfilusuario";
            Usuario usuario = await
                this.CallApiAsync<Usuario>(request, token);
            return usuario;
        }

        public async Task<List<Pedido>> GetPedidosAsync(string token)
        {
            string request = "/api/cubos/getpedidos";
            List<Pedido> pedidos =
                await this.CallApiAsync<List<Pedido>>(request, token);
            return pedidos;
        }

        public async Task NewPedidoAsync(int idcubo, int idusuario, DateTime fechapedido)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/cubos/newpedido";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Pedido pedido = new Pedido
                {
                    IdCubo = idcubo,
                    IdUsuario = idusuario,
                    FechaPedido = fechapedido
                };
                string jsonCubo =
                    JsonConvert.SerializeObject(pedido);
                StringContent content =
                    new StringContent(jsonCubo, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);

            }
        }
    }
}
