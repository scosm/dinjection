// See https://aka.ms/new-console-template for more information

using Unity;
using Inject3;
using AppLibrary;

Console.WriteLine("Creating object container...");

UnityContainer container = ObjectContainerFactory.CreateContainer();

Console.WriteLine("Starting the application...");

App app = container.Resolve<App>();

app.Run();

Console.WriteLine("Ended the application.");
