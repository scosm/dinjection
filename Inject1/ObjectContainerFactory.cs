using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using AppLibrary;

namespace Inject1
{
    /// <summary>
    /// Defines the type registrations for dependency injection and provides the object container.
    /// </summary>
    public static class ObjectContainerFactory
    {
        /// <summary>
        /// Defines the registrations (mappings) of interfaces and types.
        /// Code your mappings as needed here.
        /// </summary>
        /// <returns></returns>
        public static Container CreateContainer()
        {
            Container container = new Container();

            // Autowiring will instantiate the dependencies called for in the constructors for the registered types.

            container.Register<App, App>(Lifestyle.Transient);

            container.Register<IProductRepo, ProductRepo>(Lifestyle.Singleton);
            container.Register<IOrderService, OrderService>(Lifestyle.Singleton);

            container.Register<ILogger, TextLogger>(); // Default implementation for just one ILogger.

            // Collection of ILogger implemenations...
            // There are a couple of ways to do a collection of registration. This way provides for Lifestyle.
            // https://docs.simpleinjector.org/en/latest/using.html#auto-registering-collections
            container.Collection.Append<ILogger, TextLogger>();

            // Autowiring directly with primitve types in the constructor is disallowed: https://docs.simpleinjector.org/en/latest/using.html#resolving-instances.
            // You can create a type that holds this value (provides semantics). Or use a delegate registration (as here).
            // Or use RegisterInitializer() to configure the object with a lambda, setting a property, etc.
            string connection = "connection string";
            container.Collection.Append<ILogger>(() => new DatabaseLogger(connection), Lifestyle.Singleton);

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
                // This relies on autowiring, so that the expected constructor parameter, a TextLogger, is constructed and injected automatically.
                switch (category)
                {
                    case ProductCategory.Computer:
                        return container.GetInstance<TransactionComputer>();

                    case ProductCategory.Peripheral:
                        return container.GetInstance<TransactionPeripheral>();

                    case ProductCategory.Storage:
                        return container.GetInstance<TransactionStorage>();

                    default:
                        throw new NotImplementedException($"ProductCategory not implemented: {Enum.GetName(typeof(ProductCategory), category)}");
                }
            });

            container.Verify();

            return container;
        }

        /// <summary>
        /// A factory method for creating an order using a product.
        /// This is placed here, not the app, to avoid hardcoding new in the app, decoupling object management.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>The order with this product.</returns>
        public static IOrder OrderFactory(IProduct product)
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
