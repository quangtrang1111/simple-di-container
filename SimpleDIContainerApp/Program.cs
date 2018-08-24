using SimpleDIContainer;
using System;

namespace SimpleDIContainerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello SimpleDIContainer!");

            DIContainer.AddSingleton<IService, Service>();
            DIContainer.AddTransient<ILogger, Logger>();

            var instanceOne = DIContainer.ResolveModule<IService>();
            var instanceTwo = DIContainer.ResolveModule<IService>();

            Console.WriteLine(instanceOne == instanceTwo);

            instanceOne.DoSomething();
            instanceTwo.DoSomething();
        }
    }

    public interface IService
    {
        void DoSomething();
    }

    public class Service : IService
    {
        ILogger _logger;

        public Service(ILogger logger)
        {
            //_logger = logger;
        }

        public void DoSomething()
        {
            _logger = DIContainer.ResolveModule<ILogger>();
            _logger.Log("This is my ID:");
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class Logger : ILogger
    {
        public Guid Id = Guid.NewGuid();

        public void Log(string message)
        {
            Console.WriteLine($"{message} {Id}");
        }
    }
}
