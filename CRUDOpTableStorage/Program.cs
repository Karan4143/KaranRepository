using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CRUDOpTableStorage
{
    class Program
    {
        static CloudStorageAccount storageAccount;
        static CloudTableClient tableClient;
        static CloudTable table;

        static void Main(string[] args)
        {
            try
            {
                CreateAzureStorageTable();
                AddGuestIdentity();
                RetrieveGuestIdentity();
                UpdateGuestIdentity();
                //DeleteGuestIdentity();
                //DeleteAzureStorageTable();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void CreateAzureStorageTable()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            tableClient = storageAccount.CreateCloudTableClient();

            table = tableClient.GetTableReference("guests");

            table.CreateIfNotExists();
            Console.WriteLine("Table Created");
        }
        class GuestEntity : TableEntity
        {
            public string Name { get; set; }
            public string ContactNumber { get; set; }
            public GuestEntity() { }
            public GuestEntity(string partitionKey, string rowKey)
            {
                this.PartitionKey = partitionKey;
                this.RowKey = rowKey;
            }
        }
        private static void AddGuestIdentity()
        {
            GuestEntity guestEntity = new GuestEntity("IND", "K001");
            guestEntity.Name = "Pankaj";
            guestEntity.ContactNumber = "7292828384";
            TableOperation insertoperation = TableOperation.Insert(guestEntity);
            table.Execute(insertoperation);
            Console.WriteLine("Entity Added");
        }
        private static void RetrieveGuestIdentity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if (retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;
                Console.WriteLine($"Name:{guest.Name}  ContactNumber: {guest.ContactNumber}");
            }
            else
            {
                Console.WriteLine("Details Not Found");
            }
        }
        private static void UpdateGuestIdentity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if (retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;

                guest.ContactNumber = "9304232687";

                TableOperation updateOperation = TableOperation.Replace(guest);
                table.Execute(updateOperation);
                Console.WriteLine("Entity Updated");
            }
            else
            {
                Console.WriteLine("Details Not Updated");
            }
        }
        private static void DeleteGuestIdentity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrieveResult = table.Execute(retrieveOperation);
            if (retrieveResult.Result != null)
            {
                var guest = retrieveResult.Result as GuestEntity;
                TableOperation deleteOperation = TableOperation.Delete(guest);
                table.Execute(deleteOperation);
                Console.WriteLine("Entity Deleted");
            }
            else
            {
                Console.WriteLine("Details could not be retrieved");
            }
        }
        private static void DeleteAzureStorageTable()
        {
            table.DeleteIfExists();
            Console.WriteLine("Table Deleted");
        }

    }
}