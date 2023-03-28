using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AppLibrary;

namespace Inject2
{
    public class ObjectContainerFactory
    {
        /// <summary>
        /// Defines the registrations (mappings) of interfaces and types.
        /// Code your mappings as needed here.
        /// </summary>
        /// <returns></returns>
        public static IContainer CreateContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<App>();

            builder.RegisterType<ProductRepo>().As<IProductRepo>().SingleInstance();
            builder.RegisterType<OrderService>().As<IOrderService>().SingleInstance();

            builder.RegisterType<TextLogger>().As<ILogger>();

            // Register a type whose constructor requires a parameter: use a delegate.
            string connection = "connection string";
            builder.Register(context => new DatabaseLogger(connection)).As<ILogger>().SingleInstance();

            builder.RegisterType<Product>().As<IProduct>();
            builder.RegisterType<Order>().As<IOrder>();

            // The custom delegate for generating an order using a given product.
            builder.Register<Func<IProduct, IOrder>>(context => OrderFactory);

            builder.RegisterType<TransactionBase>().As<ITransaction>();
            builder.RegisterType<TransactionComputer>().As<TransactionComputer>();
            builder.RegisterType<TransactionPeripheral>().As<TransactionPeripheral>();
            builder.RegisterType<TransactionStorage>().As<TransactionStorage>();

            // Factory for creating a transaction.
            // This situation involves using the context so as to use Resolve() inside the factory method.
            // But there is a gotcha, in that you can't store/remember the context first name outside the factory delegate. It is not remembered from registration.
            // So, you use it to resolve again a context inside the factory.
            // Discussed: https://lizzy-gallagher.github.io/autofac-capture-componentcontext/
            builder.Register<Func<ProductCategory, ITransaction>>(context =>
            {
                IComponentContext ctx = context.Resolve<IComponentContext>();

                return (category) =>
                {
                    // This relies on autowiring, so that the expected constructor parameter, a TextLogger, is constructed and injected automatically.
                    // So, you do NOT have to do things like this:
                    // ILogger logger = context.Resolve<TextLogger>();
                    // and then manually pass in the constructor parameter value like this:
                    //    ProductCategory.Computer => context.Resolve<TransactionComputer>(new TypedParameter(typeof(TextLogger), logger)),

                    return category switch
                    {
                        ProductCategory.Computer => ctx.Resolve<TransactionComputer>(),
                        ProductCategory.Peripheral => ctx.Resolve<TransactionPeripheral>(),
                        ProductCategory.Storage => ctx.Resolve<TransactionStorage>(),
                        _ => throw new NotImplementedException($"ProductCategory not implemented: {Enum.GetName(typeof(ProductCategory), category)}"),
                    };

                };
            });

            IContainer container = builder.Build();

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
