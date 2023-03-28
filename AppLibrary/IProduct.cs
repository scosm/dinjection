using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public interface IProduct
    {
        string Id { get; set; }

        string Name { get; set; }

        ProductCategory Category { get; set; }

        DateTime CreatedDate { get; set; }

        decimal Cost { get; set; }
    }
}
