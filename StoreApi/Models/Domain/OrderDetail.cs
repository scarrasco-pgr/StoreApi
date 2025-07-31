using System.Text.Json.Serialization;

namespace StoreApi.Models.Domain
{
    public class OrderDetail
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public Guid OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; } = null!;
    }
}
