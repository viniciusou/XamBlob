using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace XamBlob
{
    //Blob Manager class
    public class BlobMan
    {
        public static BlobMan Instance { get; } = new BlobMan();

        private BlobMan()
        {
            _imagesContainer = _blobClient.GetContainerReference("images");
        }

        // Get this from the Azure Portal by clicking the "key" icon of the storage.
        const string connectionString = "azure_blob_storage_key_goes_here";

        // Create the blob client.
        CloudBlobClient _blobClient = CloudStorageAccount
            .Parse(connectionString)
            .CreateCloudBlobClient();


        CloudBlobContainer _imagesContainer;

        public async Task<List<Uri>> GetAllBlobUrisAsync()
        {
            // Not quite perfect: requires multiple queries if there are many blobs.
            var contToken = new BlobContinuationToken();
            var allBlobs = await _imagesContainer.ListBlobsSegmentedAsync(contToken).ConfigureAwait(false);
            
            var uris = allBlobs.Results.Select(b => b.Uri).ToList();

            return uris;
        }

        public async Task UploadFileAsync(string localPath)
        {
            string uniqueBlobName = Guid.NewGuid().ToString();
            uniqueBlobName += Path.GetExtension(localPath);
            var blobRef = _imagesContainer.GetBlockBlobReference(uniqueBlobName);

            // Can upload files, streams, text, ...
            await blobRef.UploadFromFileAsync(localPath).ConfigureAwait(false);
        }
    }
}
