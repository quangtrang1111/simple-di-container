using SimpleDIContainer;
using System;

namespace SimpleDIContainerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello SimpleDIContainer!");
            
            DIContainer.AddTransient<IMyService, MyService>();

            var instanceOne = DIContainer.ResolveModule<IMyService>();
            var instanceTwo = DIContainer.ResolveModule<IMyService>();

            Console.WriteLine(instanceOne == instanceTwo);
        }
    }

    public interface IMyService
    {
    }

    public class MyService : IMyService
    {
    }

    public class MyOldService
    {
    }
}
