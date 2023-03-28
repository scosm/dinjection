namespace Inject1.Test.Unit
{
    using Moq;
    using SimpleInjector;
    using AppLibrary;
    using Inject1;

    /// <summary>
    /// This test fixture encapsulates its own set of test data and mocks.
    /// Use this as the base class of the unit test class, to bring in the methods to set up test data, etc.
    /// </summary>
    public class OrderServiceFixture
    {
        private static int _productIndex = -1; // Use a static counter as closure variable.

        /// <summary>
        /// Creates the DI container to use for testing, rather than the real container and objects used by the app.
        /// </summary>
        /// <returns></returns>
        public virtual Container CreateContainer()
        {
            Container container = new Container();

            // This list of products is the mock ProductRepo collection.
            List<Product> products = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Notebook computer 13-inch",
                        Category = ProductCategory.Computer,
                        CreatedDate = DateTime.Now,
                        Cost = 1500m
                    },
                    new Product()
                    {
                        Name = "Thunderbolt docking station",
                        Category = ProductCategory.Peripheral,
                        CreatedDate = DateTime.Now,
                        Cost = 299m
                    },
                };

            // Create a mock ProductRepo
            // and then use that as the instance that implements the interface.
            Mock<IProductRepo> repoMock = new Mock<IProductRepo>();
            repoMock.Setup(p => p.GetRandomProduct()).Returns(() =>
            {
                // Just run consecutively through the products repetitively.
                _productIndex = ++_productIndex % products.Count;
                return products[_productIndex];
            });
            container.RegisterInstance<IProductRepo>(repoMock.Object);

            // Factory: The custom delegate for generating an order using a given product.
            // I can reuse the factory in the app itself. It has no dependencies to mock too.
            container.RegisterInstance<Func<IProduct, IOrder>>((product) => ObjectContainerFactory.OrderFactory(product));

            // Register the real OrderService, because that is what we have to test.
            // But the container will give it the mock ProductRepo.
            container.Register<IOrderService, OrderService>(Lifestyle.Singleton);

            container.Verify();

            return container;
        }

    }
}
