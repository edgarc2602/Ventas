using System;
namespace SistemaVentasBatia.DTOs
{
	public class Item<T> where T : notnull
	{
        public T Id { get; set; }
        public string Nom { get; set; }
        public bool Act { get; set; }
    }
}

