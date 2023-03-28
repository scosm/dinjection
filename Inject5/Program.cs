// See https://aka.ms/new-console-template for more information

using Ninject;
using Inject5;
using AppLibrary;

Console.WriteLine("Creating object container...");

using (StandardKernel container = ObjectContainerFactory.CreateContainer())
{

    Console.WriteLine("Starting the application...");

    App app = container.Get<App>();

    app.Run();

    Console.WriteLine("Ended the application.");
}
