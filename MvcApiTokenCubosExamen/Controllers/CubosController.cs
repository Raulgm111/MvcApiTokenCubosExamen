using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using MvcApiTokenCubosExamen.Filters;
using MvcApiTokenCubosExamen.Models;
using MvcApiTokenCubosExamen.Services;
using System.IO;

namespace MvcApiTokenCubosExamen.Controllers
{
    public class CubosController : Controller
    {
        private ServiceApiCubos service;
        private ServiceStorageBlobs serviceStorageBlobs;
        private string containerName = "examenazure";

        public CubosController(ServiceApiCubos service, ServiceStorageBlobs serviceStorageBlobs, IConfiguration configuration)
        {
            this.service = service;
            this.serviceStorageBlobs = serviceStorageBlobs;
        }
        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos =
                    await this.service.GetCubosAsync();
            foreach (Cubo cubo in cubos)
            {
                string blobname = cubo.Imagen;
                if (blobname != null)
                {
                    BlobContainerClient blobcontainerclient = await this.serviceStorageBlobs.GetContainersAsync(this.containerName);
                    BlobClient blocclient = blobcontainerclient.GetBlobClient(blobname);

                    BlobSasBuilder sasbuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = this.containerName,
                        BlobName = blobname,
                        Resource="b",
                        StartsOn=DateTimeOffset.UtcNow,
                        ExpiresOn=DateTime.UtcNow.AddHours(1)
                    };
                    sasbuilder.SetPermissions(BlobSasPermissions.Read);
                    var uri = blocclient.GenerateSasUri(sasbuilder);
                    cubo.Imagen = uri.ToString();

                }
            }
            return View(cubos);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string marca)
        {
            List<Cubo> cubos = await this.service.FindCuboAsync(marca);
            foreach (Cubo cubo in cubos)
            {
                string blobname = cubo.Imagen;
                if (blobname != null)
                {
                    BlobContainerClient blobcontainerclient = await this.serviceStorageBlobs.GetContainersAsync(this.containerName);
                    BlobClient blocclient = blobcontainerclient.GetBlobClient(blobname);

                    BlobSasBuilder sasbuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = this.containerName,
                        BlobName = blobname,
                        Resource = "b",
                        StartsOn = DateTimeOffset.UtcNow,
                        ExpiresOn = DateTime.UtcNow.AddHours(1)
                    };
                    sasbuilder.SetPermissions(BlobSasPermissions.Read);
                    var uri = blocclient.GenerateSasUri(sasbuilder);
                    cubo.Imagen = uri.ToString();

                }
            }
            return View(cubos);
        }

        public IActionResult NewUsuario()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewUsuario(string nombre, string email, string pass, string imagen)
        {
            await this.service.NewUsuarioAsync(nombre, email, pass, imagen);
            return RedirectToAction("Index");
        }

        public IActionResult NewCubo()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> NewCubo(string nombre, string marca, string imagen, int precio)
        //{
        //    await this.service.NewCuboAsync(nombre, marca, imagen, precio);
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public async Task<IActionResult> NewCubo(string nombre, string marca, string imagen, int precio, IFormFile file)
        {
            string blobName = file.FileName;
            imagen = blobName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceStorageBlobs.UploadBlobAsync(this.containerName, blobName, stream);
            }
            await this.service.NewCuboAsync(nombre, marca, imagen, precio);
            return RedirectToAction("Index");
        }

        [AuthorizeCubos]
        public async Task<IActionResult> Perfil()
        {
            string token = HttpContext.Session.GetString("TOKEN");
            Usuario usuario = await this.service.GetPerfilUsuarioAsync(token);
            return View(usuario);
        }

        [AuthorizeCubos]
        public async Task<IActionResult> GetPedidos()
        {
            string token =
                HttpContext.Session.GetString("TOKEN");
            if (token == null)
            {
                ViewData["MENSAJE"] = "Debe realizar Login para visualizar datos";
                return View();
            }
            else
            {
                List<Pedido> pedidos =
                    await this.service.GetPedidosAsync(token);
                return View(pedidos);
            }
        }
    }
}
