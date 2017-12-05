using Discord;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordClient
{
    public static class ExceptionUploader
    {
        public static void UploadException(string exceptionMessage, string stacktrace)
        {
#if LOG_EXCEPTIONS
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Time of Crash: " + DateTime.UtcNow.ToString());
            sb.AppendLine("Application Name: " + DeviceInfo.ApplicationName);
            sb.AppendLine("Application Version: " + DeviceInfo.ApplicationVersion);
            sb.AppendLine("Device Manufacturer: " + DeviceInfo.DeviceManufacturer);
            sb.AppendLine("Device Model: " + DeviceInfo.DeviceModel);
            sb.AppendLine("Architecture: " + DeviceInfo.SystemArchitecture);
            sb.AppendLine("System Family: " + DeviceInfo.SystemFamily);
            sb.AppendLine("System Version: " + DeviceInfo.SystemVersion);
            sb.AppendLine("Connection State: " + GnarlyClient.Instance.DiscordClient.State.ToString());
            sb.AppendLine();

            sb.AppendLine("Exception Message: " + exceptionMessage);
            sb.AppendLine("Stack Trace:");
            sb.AppendLine(stacktrace);

            string excetpionFileName = Guid.NewGuid().ToString() + ".txt";
            string errorReport = sb.ToString();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(BlobConnection);
            if (storageAccount != null)
            {
                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference(ExceptionContainer);
                if (container != null)
                {
                   var existsTask = container.ExistsAsync();
                    bool exists = existsTask.Wait(TimeOut);
                    if (exists)
                    {
                        // Retrieve reference to a blob named "myblob".
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(excetpionFileName);
                        var upload =  blockBlob.UploadTextAsync(errorReport);
                        upload.Wait(TimeOut);
                    }
                }
            }
#endif
        }

        private const int TimeOut = 10000;
        private const string BlobConnection = "PrivateToGnarlysoft"; //This is a private key that allows us to upload the excpetion data to a server. This will not be shared in the open source project.
        private const string ExceptionContainer = "exceptions";
    }
}
