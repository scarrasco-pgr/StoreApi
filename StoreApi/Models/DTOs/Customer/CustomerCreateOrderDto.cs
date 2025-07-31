namespace StoreApi.Models.DTOs.Customer
{
    public class OrderProductDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CustomerCreateOrderDto
    {
        public List<OrderProductDto> Items { get; set; } = [];
    }
}