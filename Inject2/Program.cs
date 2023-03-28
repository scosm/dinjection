// See https://aka.ms/new-console-template for more information

using Autofac;
using Inject2;
using AppLibrary;

using (IContainer container = ObjectContainerFactory.CreateContainer())
{
    Console.WriteLine("Creating object container...");

    Console.WriteLine("Starting the application...");

    App app = container.Resolve<App>();

    app.Run();

    Console.WriteLine("Ended the application.");
}
    