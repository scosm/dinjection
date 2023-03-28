using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class OrderService : IOrderService
    {
        private IProductRepo _productRepo;

        /// <summary>
        /// An internal cache of orders.
        /// </summary>
        private Queue<IOrder> _orders;

        // We inject an instance of this delegate, to allow us to control
        // the instantiation of orders in this service.
        // https://docs.simpleinjector.org/en/latest/howto.html#register-factory-delegates
        private Func<IProduct, IOrder> _orderFactory;

        public OrderService(IProductRepo productRepo, Func<IProduct, IOrder> orderFactory)
        {
            _productRepo = productRepo;

            _orders = new Queue<IOrder>();
            _orderFactory = orderFactory;

            UpdateQueue();
        }

        public int OrderCount => _orders.Count;

        public IOrder GetNextOrder()
        {
            UpdateQueue();
            return _orders.Dequeue();
        }

        public void UpdateQueue()
        {
            // Add more orders if the queue is near empty.
            if (OrderCount <= 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    IProduct product = _productRepo.GetRandomProduct();
                    IOrder order = _orderFactory(product);
                    _orders.Enqueue(order);
                }
            }
        }

    }
}
