using Microsoft.Extensions.DependencyInjection;
using SimpleDIContainer;
using System;

namespace SimpleDIContainerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello SimpleDIContainer!");

            //DIContainer.AddSingleton<IService, Service>();
            //DIContainer.AddSingleton<ILogger, Logger>();

            //var instanceOne = DIContainer.ResolveModule<IService>();
            //var instanceTwo = DIContainer.ResolveModule<IService>();

            //Console.WriteLine(instanceOne == instanceTwo);

            //instanceOne.DoSomething();
            //instanceTwo.DoSomething();

            var services = new ServiceCollection();
            services.AddScoped<ILogger, Logger>();
            var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<IServiceScopeFactory>();

            var scope1 = factory.CreateScope();
            var instance1 = scope1.ServiceProvider.GetService<ILogger>();
            var instance2 = scope1.ServiceProvider.GetService<ILogger>();

            var scope2 = factory.CreateScope();
            var instance3 = scope2.ServiceProvider.GetService<ILogger>();

            Console.WriteLine(instance1 == instance2);
            Console.WriteLine(instance2 == instance3);
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
            _logger = logger;
        }

        public void DoSomething()
        {
            //_logger = DIContainer.ResolveModule<ILogger>();
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
