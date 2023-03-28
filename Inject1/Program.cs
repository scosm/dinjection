// See https://aka.ms/new-console-template for more information

using SimpleInjector;
using Inject1;
using AppLibrary;

Console.WriteLine("Creating object container...");

Container container = ObjectContainerFactory.CreateContainer();

Console.WriteLine("Starting the application...");

App app = container.GetInstance<App>();

app.Run();

Console.WriteLine("Ended the application.");
