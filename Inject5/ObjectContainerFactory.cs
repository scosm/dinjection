using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using AppLibrary;

namespace Inject5
{
    public class ObjectContainerFactory
    {
        public static StandardKernel CreateContainer()
        {
            StandardKernel kernel = new StandardKernel();

            kernel.Bind<App>().To<App>();

            kernel.Bind<IProductRepo>().To<ProductRepo>().InSingletonScope();
            kernel.Bind<IOrderService>().To<OrderService>().InSingletonScope();

            // This is how a collection of implementations of the same interface is registered.
            // See: https://github.com/ninject/Ninject/wiki/Multi-injection
            // kernel.GetAllInstances() returns IEnumberable, and as you iterate of this, then each object is instantiated.
            // But to resolve only one of these, you use the .Named() method to assign a string name, which you cite when you need to resolve it.
            // https://github.com/ninject/Ninject/wiki/Contextual-Binding
            // So, then you have to use the [Named("name")] attribute on the parameter in the constructor signature of the target where this is used.
            // HOWEVER, this introduces a dependency on the DI container in the app code, which is unacceptable. (It also requires "magic strings" for naming these bindings.)
            // So, instead, you can use .WhenInjectedInto(type) on the binding to say where to use the object.
            kernel.Bind<ILogger>().To<TextLogger>().WhenInjectedInto(typeof(TransactionBase)); // This is for the single logger situation. This will not be included when getting the IEnumerable.
            kernel.Bind<ILogger>().To<TextLogger>().Named("TextLogger"); // Need this to have this logger when getting the IEnumerable.
            string connection = "connection string";
            // NOTE the not-type-safe naming of the constructor parameter based on the code.
            kernel.Bind<ILogger>().To<DatabaseLogger>().Named("DatabaseLogger").WithConstructorArgument("connection", connection);

            kernel.Bind<IProduct>().To<Product>();
            kernel.Bind<IOrder>().To<Order>();

            // Factory: The custom delegate for generating an order using a given product.
            kernel.Bind<Func<IProduct, IOrder>>().ToMethod((context) => (product) => OrderFactory(product));

            kernel.Bind<ITransaction>().To<TransactionBase>();
            kernel.Bind<TransactionComputer>().To<TransactionComputer>();
            kernel.Bind<TransactionPeripheral>().To<TransactionPeripheral>();
            kernel.Bind<TransactionStorage>().To<TransactionStorage>();

            kernel.Bind<Func<ProductCategory, ITransaction>>().ToMethod((context) => (category) =>
            {
                // TODO: Problem of first/default object in collection in Ninject. Use contextual binding. https://github.com/ninject/Ninject/wiki/Contextual-Binding

                // This normally could rely on autowiring, so that the expected constructor parameter, a TextLogger, would be constructed and injected automatically.
                // But with the collection of ILoggers, we have named registrations. We don't have an unnamed logger. Or do we??
                // In fact, we can leave our "default" logger (TextLogger) unnamed. We don't need the name generally, except for sorting the collection. (We aren't needing sorting at present, but it works still.)
                // So, now we can rely still on autowiring with the TextLogger as the default single logger resolution, when no name is specified.
                return category switch
                {
                    ProductCategory.Computer => kernel.Get<TransactionComputer>(),
                    ProductCategory.Peripheral => kernel.Get<TransactionPeripheral>(),
                    ProductCategory.Storage => kernel.Get<TransactionStorage>(),
                    _ => throw new NotImplementedException($"ProductCategory not implemented: {Enum.GetName(typeof(ProductCategory), category)}"),
                };
            });

            return kernel;
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
