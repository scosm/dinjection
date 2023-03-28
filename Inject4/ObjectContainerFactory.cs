using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using AppLibrary;

namespace Inject4
{
    public static class ObjectContainerFactory
    {
        public static ServiceContainer CreateContainer()
        {
            // Since singletons are per container, we need to dispose of the container. But do so in the Main(), so we still have the container undisposed during app operation (cf. Autofac).
            ServiceContainer container = new LightInject.ServiceContainer();

            // Autowiring will instantiate the dependencies called for in the constructors for the registered types.
            container.Register<App, App>(new PerContainerLifetime()); // Singleton.
            // Or do this: container.RegisterSingleton<App, App>();

            container.Register<IProductRepo, ProductRepo>(new PerContainerLifetime());
            container.RegisterSingleton<IOrderService, OrderService>();

            // Collection of ILogger implemenations...
            container.Register<ILogger, TextLogger>(); // Default implementation for just one ILogger, because its ordering string name will sort this as first. Seems still to sort (first) when no name is given.

            // Autowiring directly with primitve types in the constructor is disallowed: https://docs.simpleinjector.org/en/latest/using.html#resolving-instances.
            // You can create a type that holds this value (provides semantics). Or use a delegate registration (as here).
            // Or use RegisterInitializer() to configure the object with a lambda, setting a property, etc.
            string connection = "connection string";
            container.Register<ILogger>((factory) => new DatabaseLogger(connection), "2", new PerContainerLifetime());

            container.Register<IProduct, Product>();
            container.Register<IOrder, Order>();

            // Factory: The custom delegate for generating an order using a given product.
            container.RegisterInstance<Func<IProduct, IOrder>>((product) => OrderFactory(product));

            container.Register<ITransaction, TransactionBase>();
            container.Register<TransactionComputer, TransactionComputer>();
            container.Register<TransactionPeripheral, TransactionPeripheral>();
            container.Register<TransactionStorage, TransactionStorage>();

            // Factory for creating a transaction.
            // Factories differ from the rest of app code in that they can involve a dependency on the DI container.
            // That is, the main app code does not reference the container.
            // But note that we could still use new here too, since again we are not in the app code itself. This is the code where we can say how things are instantiated.
            // However, it is cleaner to leave it to the container, so there is only one place when possible where we define how a type is instantiated.
            container.RegisterInstance<Func<ProductCategory, ITransaction>>((category) =>
            {
                // This normally could rely on autowiring, so that the expected constructor parameter, a TextLogger, would be constructed and injected automatically.
                // But with the collection of ILoggers, we have named registrations. We don't have an unnamed logger. Or do we??
                // In fact, we can leave our "default" logger (TextLogger) unnamed. We don't need the name generally, except for sorting the collection. (We aren't needing sorting at present, but it works still.)
                // So, now we can rely still on autowiring with the TextLogger as the default single logger resolution, when no name is specified.
                return category switch
                {
                    ProductCategory.Computer => container.GetInstance<TransactionComputer>(),
                    ProductCategory.Peripheral => container.GetInstance<TransactionPeripheral>(),
                    ProductCategory.Storage => container.GetInstance<TransactionStorage>(),
                    _ => throw new NotImplementedException($"ProductCategory not implemented: {Enum.GetName(typeof(ProductCategory), category)}"),
                };
            });

            return container;
        }

        /// <summary>
        /// A factory method for creating an order using a product.
        /// This is placed here, not the app, to avoid hardcoding new in the app, decoupling object management.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>The order with this product.</returns>
        private static IOrder OrderFactory(IProduct product)
        {
            // This shows that maybe it would be better to let Order have a default constructor (no product)
            // and then let Product be a property that is assigned. So then we could use the container
            // to create the Order (not new). Cf. Transaction registration.
            IOrder order = new Order(product)
            {
                OrderDate = DateTime.Now
            };

            return order;
        }

    }
}
