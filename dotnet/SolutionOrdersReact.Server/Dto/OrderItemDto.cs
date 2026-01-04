namespace SolutionOrdersReact.Server.Dto
{

    /// <summary>
    /// DTO dla pozycji zamówienia
    /// </summary>
    public class OrderItemDto
    {
        public int IdOrderItem { get; set; }
        public int IdItem { get; set; }
        public string? ItemName { get; set; }
        public string? ItemCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string? UnitName { get; set; }
        public bool IsActive { get; set; }
    }
}