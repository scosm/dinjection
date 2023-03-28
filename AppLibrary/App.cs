using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class App
    {
        // Some basic design principles:
        // No hardcoded use of new to instantiate dependencies.
        // No reference to the object container, e.g., using attributes or explicit object resolution out of the container.
        // Usually use an interface for objects, though sometimes it is fine to register the class itself without an interface.
        // Avoid property-based injection, because of weaknesses, which as described in SimpleInjector documentation.
        // This refers to injecting a downstream dependency, into an object when it is resolved, via a property of the object rather than the constructor.
        // But this does not rule out having a factory that accepts a parameter that is then used to assign this value to a property of the object being created.
        // In the Inject{n} class, prefer type-safe and simple code, because we don't want bugs in that area where we use the DI container directly and unit testing because difficult.

        private IOrderService _orderService;

        Func<ProductCategory, ITransaction> _transactionFactory;

        private IEnumerable<ILogger> _loggers;

        public App(IOrderService orderService, Func<ProductCategory, ITransaction> transactionFactory, IEnumerable<ILogger> loggers)
        {
            // Note that OrderService depends on an IProductRepo, but the container will provide thils.
            // The App does not directly depend on IProductRepo.
            _orderService = orderService;
            _transactionFactory = transactionFactory;
            _loggers = loggers;
        }

        public void Run()
        {
            int n = 0;
            // Get orders and process them.
            while (_orderService.OrderCount > 0)
            {
                IOrder order = _orderService.GetNextOrder();

                // Log the activity to different places.
                foreach (ILogger logger in _loggers)
                {
                    logger.Info($"{n}. Order: {order.OrderDate}, {order.Product.Name}, {order.Product.Cost}");
                }

                // Create a transaction object and process order.
                ProductCategory category = order.Product.Category;
                ITransaction transaction = _transactionFactory(category);
                transaction.Order = order;

                transaction.Execute();

                Console.WriteLine();

                n++;
                if (n > 100)
                    break;
            }

        }

    }
}
