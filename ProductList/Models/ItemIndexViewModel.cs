using System.Collections.Generic;



namespace ProductList.Models
{
    public class ItemIndexViewModel
    {
        public IEnumerable<Item> Items { get; set; }
        public ItemViewModel ItemForm { get; set; }
    }
}