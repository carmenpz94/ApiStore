namespace ApiStore.DTOs
{
    public class ProductResponse
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public decimal Precio { get; set; }

        public int CategoriaId { get; set; }

        public int Stock { get; set; }

        public string? Imagen { get; set; }

        public virtual CategoryResponse Categoria { get; set; } = null!;
    }

    public class ProductRequest
    {
        public int ProductId;

        //public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public decimal Precio { get; set; }

        public int CategoriaId { get; set; }

        public int Stock { get; set; }

        public string? Imagen { get; set; }

        public virtual CategoryRequest Categoria { get; set; } = null!;
        public int Id { get; set; }
    }
}
