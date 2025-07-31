namespace StoreApi.Models.DTOs.Order
{
    public class UpdateOrderDto
    {
        public DateTime OrderPlaced { get; set; }
        public DateTime? OrderFulfilled { get; set; }
        public Guid CustomerId { get; set; }
    }
}
