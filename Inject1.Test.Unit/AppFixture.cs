namespace Inject1.Test.Unit
{
    using Moq;
    using SimpleInjector;
    using AppLibrary;
    using Inject1;

    public class AppFixture
    {
        private static int _orderIndex = -1;
        private static int _rawIndex = 0;

        public virtual Container CreateContainer()
        {
            Container container = new Container();

            // This list of products is the mock ProductRepo collection.
            List<Order> orders = new List<Order>()
                {
                    new Order(new Product()
                        {
                            Name = "Notebook computer 13-inch",
                            Category = ProductCategory.Computer,
                            CreatedDate = DateTime.Now,
                            Cost = 1500m
                        }),
                    new Order(new Product()
                        {
                            Name = "Thunderbolt docking station",
                            Category = ProductCategory.Peripheral,
                            CreatedDate = DateTime.Now,
                            Cost = 299m
                        })
                };

            // IOrderService.
            Mock<IOrderService> orderServiceMock = new Mock<IOrderService>();
            orderServiceMock.Setup(s => s.GetNextOrder()).Returns(() =>
            {
                _orderIndex = ++_orderIndex % orders.Count;
                _rawIndex++;
                return orders[_orderIndex];
            });
            // OrderCount will decrement once for order we take using GetNextOrder(), until we stop at 0.
            orderServiceMock.Setup(s => s.OrderCount).Returns(() =>
            {
                int count = Math.Max(orders.Count - _rawIndex, 0);
                return count;
            });
            container.RegisterInstance<IOrderService>(orderServiceMock.Object);

            // IProductRepo.
            // Since we mock OrderService, we don't use IProductRepo.GetRandomProduct().
            // But in principle we might use the other methods later....
            Mock<IProductRepo> productRepoMock = new Mock<IProductRepo>();
            //repo.Setup(p => p.ProductsByName).Returns(() => {
            //    throw new NotImplementedException("A mock implementation of ProductsByName() is not yet specified.");
            //});
            container.RegisterInstance<IProductRepo>(productRepoMock.Object);

            // OrderFactory: The custom delegate for generating an order using a given product.
            // I can reuse the factory in the app itself. It has no dependencies to mock too.
            container.RegisterInstance<Func<IProduct, IOrder>>((product) => ObjectContainerFactory.OrderFactory(product));

            // ITransaction.
            // For simplicity, we will provide one, same transaction type for all categories, as a mock.
            // It will take a mock logger too (automatically from container).
            Mock<ITransaction> transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(t => t.Execute()).Verifiable();
            container.RegisterInstance<ITransaction>(transactionMock.Object);

            // Factory for creating a transaction.
            container.RegisterInstance<Func<ProductCategory, ITransaction>>((category) =>
            {
                switch (category)
                {
                    case ProductCategory.Computer:
                    case ProductCategory.Peripheral:
                    case ProductCategory.Storage:
                        return container.GetInstance<ITransaction>();

                    default:
                        throw new NotImplementedException($"ProductCategory not implemented: {Enum.GetName(typeof(ProductCategory), category)}");
                }
            });

            // ILogger.
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            loggerMock.Setup(m => m.Info(It.IsAny<string>())).Verifiable();
            container.RegisterInstance<ILogger>(loggerMock.Object); // Default single-item registration.
            container.Collection.AppendInstance<ILogger>(loggerMock.Object); // Register IEnumerable<ILogger>.

            container.Register<App, App>(Lifestyle.Transient);

            // Add mocks whose method calls we want to verify in the unit tests, that they occurred.
            container.RegisterInstance<Mock<IOrderService>>(orderServiceMock);
            container.RegisterInstance<Mock<ITransaction>>(transactionMock);

            container.Verify();

            return container;
        }
    }
}
