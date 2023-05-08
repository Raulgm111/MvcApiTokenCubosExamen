using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MvcApiTokenCubosExamen.Models;

namespace MvcApiTokenCubosExamen.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        //METODO PARA MOSTRAR TODOS LOS CONTENEDORES
        public async Task<BlobContainerClient> GetContainersAsync(string containername)
        {
            BlobContainerClient container = this.client.GetBlobContainerClient(containername);
            return container;
        }
        //METODO PARA CREAR CONTENEDORES
        public async Task CreateContainerAsync(string containerName)
        {
            //containerName = containerName.ToLower();
            //DEBEMOS INDICAR EL NOMBRE DEL CONTENEDOR Y SU TIPO
            //DE ACCESO
            await this.client.CreateBlobContainerAsync
                (containerName, PublicAccessType.Blob);
        }

        //METODO PARA ELIMINAR CONTENEDORES
        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }

        //METODO PARA RECUPERAR TODOS LOS BLOBS
        public async Task<List<BlobModel>> GetBlobsAsync
            (string containerName)
        {
            //RECUPERAMOS UN CLIENT DEL CONTAINER
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<BlobModel> blobModels = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                //NECESITAMOS UN BLOB CLIENT PARA VISUALIZAR MAS 
                //CARACTERISTICAS DEL OBJETO
                BlobClient blobClient =
                    containerClient.GetBlobClient(item.Name);
                BlobModel model = new BlobModel();
                model.Nombre = item.Name;
                model.Contenedor = containerName;
                model.Url = blobClient.Uri.AbsoluteUri;
                blobModels.Add(model);
            }
            return blobModels;
        }

        //METODO PARA ELIMINAR BLOBS
        public async Task DeleteBlobAsync
            (string containerName, string blobName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }

        //METODO PARA SUBIR UN BLOB
        public async Task UploadBlobAsync
            (string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }
    }
}
