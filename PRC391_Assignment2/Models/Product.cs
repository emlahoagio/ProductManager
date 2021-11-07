using Amazon.DynamoDBv2.DataModel;

namespace PRC391_Assignment2.Models
{
    [DynamoDBTable("Product_DoTrongMinhQuan")]
    public class Product
    {
        [DynamoDBHashKey]
        public int ProductId { get; set; }
        [DynamoDBRangeKey]
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
