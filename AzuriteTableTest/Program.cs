using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzuriteTableTest
{
    class TestTableEntity : TableEntity
    {
        public double Value { get; set; }

        public TestTableEntity()
        {
        }

        public TestTableEntity(double value)
        {
            Value = value;
        }
    }


    class Program
    {
        static async Task Main(string[] args)
        {
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("testtable");

            var data = new TestTableEntity(1);
            data.PartitionKey = "PARTITION";
            data.RowKey = "ROW";

            await table.CreateIfNotExistsAsync();
            var insertResult = await table.ExecuteAsync(TableOperation.InsertOrReplace(data));
            var queryResult = await table.ExecuteQuerySegmentedAsync<TestTableEntity>(
                new TableQuery<TestTableEntity>().Take(1),
                null
                );

            var insertedData = insertResult.Result as TestTableEntity;
            var retrievedData = queryResult.Results.Single();
            Console.WriteLine($"Insert: {insertResult.HttpStatusCode}, {insertedData.PartitionKey}, {insertedData.RowKey}, {insertedData.Value}");
            Console.WriteLine($"Query: {retrievedData.PartitionKey}, {retrievedData.RowKey}, {retrievedData.Value}");

            if (insertedData.Value != retrievedData.Value)
            {
                throw new Exception("Values differ");
            }
        }
    }
}
