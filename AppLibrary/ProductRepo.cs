using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AppLibrary
{
    /// <summary>
    /// Database of all the available products.
    /// This could be a REST API to a database or maybe just an accessor via querying.
    /// </summary>
    public class ProductRepo : IProductRepo
    {
        private List<Product> _products;

        public ProductRepo()
        {
            // Make up a pseudo-repo to mimic a backend database.
            _products = new List<Product>();

            _products.Add(new Product()
            {
                Name = "Notebook computer 15-inch",
                Category = ProductCategory.Computer,
                CreatedDate = DateTime.Now,
                Cost = 2200m
            });
            _products.Add(new Product()
            {
                Name = "Thunderbolt docking station",
                Category= ProductCategory.Peripheral,
                CreatedDate = DateTime.Now,
                Cost = 259m
            });
            _products.Add(new Product()
            {
                Name = "32-inch external monitor",
                Category = ProductCategory.Peripheral,
                CreatedDate = DateTime.Now,
                Cost = 425m
            });
            _products.Add(new Product()
            {
                Name = "2TB flash drive",
                Category = ProductCategory.Storage,
                CreatedDate = DateTime.Now,
                Cost = 195m
            });
            _products.Add(new Product()
            {
                Name = "USB 3 hub",
                Category = ProductCategory.Storage,
                CreatedDate = DateTime.Now,
                Cost = 36m
            });
            _products.Add(new Product()
            {
                Name = "Wireless mouse",
                Category = ProductCategory.Peripheral,
                CreatedDate = DateTime.Now,
                Cost = 29m
            });
            _products.Add(new Product()
            {
                Name = "Wireless keyboard",
                Category = ProductCategory.Peripheral,
                CreatedDate = DateTime.Now,
                Cost = 65m
            });
        }

        /// <summary>
        /// Gets a list of products based on a regex pattern for the name.
        /// </summary>
        /// <param name=""></param>
        /// <returns>List of products with conforming name.</returns>
        public List<Product> ProductsByName(string pattern)
        {
            Regex nameRegex = new Regex(pattern);
            return _products.Where(p => nameRegex.IsMatch(p.Name)).ToList();
        }

        /// <summary>
        /// Gets the latest n products.
        /// </summary>
        /// <param name="n"></param>
        /// <returns>List of n most recent products according to CreatedDate.</returns>
        public List<Product> LatestProducts(int n)
        {
            return _products.OrderByDescending(p => p.CreatedDate).Take(n).ToList();
        }

        /// <summary>
        /// Retrieves a random product from the repo.
        /// </summary>
        /// <returns>Some product or other.</returns>
        public IProduct GetRandomProduct()
        {
            Random random = new Random();
            int index = random.Next(_products.Count);
            return _products[index];
        }

    }
}
