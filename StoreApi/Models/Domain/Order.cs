namespace StoreApi.Models.Domain
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderPlaced { get; set; }
        public DateTime? OrderFulfilled { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<OrderDetail> OrderItems { get; set; } = [];
    }
}
