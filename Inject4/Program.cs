// See https://aka.ms/new-console-template for more information

using LightInject;
using Inject4;
using AppLibrary;

Console.WriteLine("Creating object container...");

using (ServiceContainer container = ObjectContainerFactory.CreateContainer())
{

    Console.WriteLine("Starting the application...");

    App app = container.GetInstance<App>();

    app.Run();

    Console.WriteLine("Ended the application.");
}
