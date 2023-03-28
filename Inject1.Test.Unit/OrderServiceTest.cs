namespace Inject1.Test.Unit
{
    using Xunit;
    using SimpleInjector;
    using AppLibrary;

    public class OrderServiceTest : OrderServiceFixture
    {
        [Fact]
        public void GetNextOrder_Happy()
        {
            Container container = CreateContainer();

            IOrderService orderService = container.GetInstance<IOrderService>();

            IOrder order = orderService.GetNextOrder();
            Assert.NotNull(order);
            Assert.Equal("Notebook computer 13-inch", order.Product.Name);

            order = orderService.GetNextOrder();
            Assert.NotNull(order);
            Assert.Equal("Thunderbolt docking station", order.Product.Name);
        }

    }
}