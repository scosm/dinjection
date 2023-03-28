using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public interface IProductRepo
    {
        List<Product> ProductsByName(string pattern);

        List<Product> LatestProducts(int count);

        IProduct GetRandomProduct();
    }
}
