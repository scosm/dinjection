using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class Product : IProduct
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ProductCategory Category { get; set; }

        public DateTime CreatedDate { get; set; }

        public decimal Cost { get; set; }

        public Product()
        {
            Id = Guid.NewGuid().ToString();
            Name = "";
        }

    }
}
