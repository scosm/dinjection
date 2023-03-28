namespace Inject1.Test.Unit
{
    using Xunit;
    using Moq;
    using SimpleInjector;
    using AppLibrary;
    using Inject1;

    public class AppTest : AppFixture
    {
        [Fact]
        public void Run_Happy()
        {
            Container container = CreateContainer();

            App app = container.GetInstance<App>();
            Mock<IOrderService> orderServiceMock = container.GetInstance<Mock<IOrderService>>();
            Mock<ITransaction> transactionMock = container.GetInstance<Mock<ITransaction>>();

            app.Run();

            orderServiceMock.Verify((service) => service.GetNextOrder(), Times.Exactly(2));
            transactionMock.Verify((transaction) => transaction.Execute(), Times.Exactly(2));

        }
    }
}
