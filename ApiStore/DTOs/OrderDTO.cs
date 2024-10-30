using ApiStore.Models;

namespace ApiStore.DTOs
{
    public class OrderResponse
    {
        public int Id { get; set; }

        public DateOnly FechaPedido { get; set; }

        public string EstadoPedido { get; set; } = null!;

        public int UserId { get; set; }

        //public virtual User User { get; set; } = null!;

        //public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
    public class OrderRequest
    {
        //public int Id { get; set; }

        public DateOnly FechaPedido { get; set; }

        public string EstadoPedido { get; set; } = null!;

        public int UserId { get; set; }
        public int ClienteId { get; set; }

        //public virtual User User { get; set; } = null!;

        //public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }








}
