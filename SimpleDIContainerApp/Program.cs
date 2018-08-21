using SimpleDIContainer;
using System;

namespace SimpleDIContainerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello SimpleDIContainer!");

            DIContainer.AddSingleton<ILogger, Logger>();
            var service = new Service(DIContainer.ResolveModule<ILogger>());
            service.DoSomething();

            DIContainer.AddTransient<IService, Service>();
            var instanceOne = DIContainer.ResolveModule<IService>();
            var instanceTwo = DIContainer.ResolveModule<IService>();
            Console.WriteLine(instanceOne == instanceTwo);
        }
    }

    public interface IService
    {
    }

    public class Service : IService
    {
        private readonly ILogger _logger;

        public Service(ILogger logger)
        {
            _logger = logger;
        }

        public void DoSomething()
        {
            _logger.Log("I'm doing a stupid thing");
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
