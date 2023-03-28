using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class Order : IOrder
    {
        public Order(IProduct product)
        {
            Id = Guid.NewGuid().ToString();
            Product = product;
            // OrderDate will be set by property during processing.
        }

        public string Id { get; set; }

        public IProduct Product { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
