using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;
using AppLibrary;

namespace Inject3
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
        public static UnityContainer CreateContainer()
        {
            UnityContainer container = new UnityContainer();

            // Autowiring will instantiate the dependencies called for in the constructors for the registered types.
            container.RegisterType<App, App>(TypeLifetime.Singleton);

            container.RegisterType<IProductRepo, ProductRepo>(TypeLifetime.Singleton);
            container.RegisterType<IOrderService, OrderService>(TypeLifetime.Singleton);

            // Collection of ILogger implemenations. To register a set of implementations you have to give each one a name.
            // Unity docs:
            //   When you want to obtain a list of all the registered objects of a specific type, IPrinter in this case,
            //   you can use the array T[] or IEnumerable<T> of that type.
            //   The difference between array and enumerable resolution is that array only returns named (nondefault name) registrations
            //   where enumerable always returns all, named and unnamed registrations.
            // This seems like a design flaw to me, because we don't expect a (latent) semantic shift in usage betwene IEnumerable<T> and T[].
            // The App takes: IEnumerable<ILogger> loggers .
            // And we don't want to change/break the app code for this and use an array. But this now introduces a dependency on how the DI framework is designed to work, the semantics of its API.
            // Therefore, do not define the default (no-name) registration. But when only the TextLogger is needed, try to resolve only for that, explicitly, by its name, here in factory below.
            //container.RegisterType<ILogger, TextLogger>(); // Default implementation for just one ILogger.

            // http://unitycontainer.org/tutorials/resolution/collections.html
            container.RegisterType<ILogger, TextLogger>("TextLogger");
            string connection = "connection string";
            container.RegisterInstance<ILogger>("DatabaseLogger", new DatabaseLogger(connection), InstanceLifetime.Singleton);

            container.RegisterType<IProduct, Product>();
            container.RegisterType<IOrder, Order>();

            // Factory: The custom delegate for generating an order using a given product.
            container.RegisterInstance<Func<IProduct, IOrder>>((product) => OrderFactory(product));

            container.RegisterType<ITransaction, TransactionBase>();
            container.RegisterType<TransactionComputer, TransactionComputer>();
            container.RegisterType<TransactionPeripheral, TransactionPeripheral>();
            container.RegisterType<TransactionStorage, TransactionStorage>();

            // Factory for creating a transaction.
            // Factories differ from the rest of app code in that they can involve a dependency on the DI container.
            // That is, the main app code does not reference the container.
            // But note that we could still use new here too, since again we are not in the app code itself. This is the code where we can say how things are instantiated.
            // However, it is cleaner to leave it to the container, so there is only one place when possible where we define how a type is instantiated.
            container.RegisterInstance<Func<ProductCategory, ITransaction>>((category) =>
            {
                // A complication with Unity, unlike SimpleInjector or even Autofac.
                // This normally could rely on autowiring, so that the expected constructor parameter, a TextLogger, would be constructed and injected automatically.
                // But with Unity, because of the above need to register multiple implementations of ILogger, we have to get the specific logger of interest here,
                // since we want only one. So, we grab the one we want by its name and pass that is to the constructor.
                // BUT unfortunately, the basic solution requires naming the parameter to override with a string name, which is not type safe.
                ILogger logger = container.Resolve<ILogger>("TextLogger"); // The name used above in registration the TextLogger.

                // We have to specify a parameter override object to pass in this logger explicitly, instead of relying on autowiring.
                // Below, .OnType<TextLogger>(); // Don't add this OnType(), because it is not necessary to set the type to constrain the override to...
                // and it causes an exception that we don't have a constructor for ILogger.
                ParameterOverride parameterOverride = new ParameterOverride("logger", logger);

                return category switch
                {
                    ProductCategory.Computer => container.Resolve<TransactionComputer>(parameterOverride),
                    ProductCategory.Peripheral => container.Resolve<TransactionPeripheral>(parameterOverride),
                    ProductCategory.Storage => container.Resolve<TransactionStorage>(parameterOverride),
                    _ => throw new NotImplementedException($"ProductCategory not implemented: {Enum.GetName(typeof(ProductCategory), category)}"),
                };
            });

            // Unity does not have a container.Verify() or .Build() method to use here.

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
