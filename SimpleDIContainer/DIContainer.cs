using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleDIContainer
{
    enum Scope
    {
        Singletone,
        Transient
    }

    public class DIContainer
    {
        private static readonly Dictionary<Type, (Scope, object)> ResgisteredModules = new Dictionary<Type, (Scope, object)>();

        public static void AddSingleton<TInterface, TModule>()
        {
            SetModule(typeof(TInterface), typeof(TModule), Scope.Singletone);
        }

        public static void AddTransient<TInterface, TModule>()
        {
            SetModule(typeof(TInterface), typeof(TModule), Scope.Transient);
        }

        public static T ResolveModule<T>()
        {
            return (T)GetModule(typeof(T));
        }

        private static void SetModule(Type interfaceType, Type moduleType, Scope scope)
        {
            if (ResgisteredModules.ContainsKey(interfaceType))
            {
                return;
            }

            if (!interfaceType.IsAssignableFrom(moduleType))
            {
                throw new Exception($"Module {interfaceType.FullName} wasn't implemented");
            }


            var firstConstructor = moduleType.GetConstructors().FirstOrDefault();
            switch (scope)
            {
                case Scope.Singletone:
                    object module = getModuleInstance(firstConstructor);
                    ResgisteredModules.Add(interfaceType, (scope, module));
                    break;
                case Scope.Transient:
                    ResgisteredModules.Add(interfaceType, (scope, firstConstructor));
                    break;
                default:
                    break;
            }
        }

        private static object getModuleInstance(ConstructorInfo constructor)
        {
            object module = null;

            if (!constructor.GetParameters().Any())
            {
                module = constructor.Invoke(null); // new Object instance
            }
            else
            {
                var constructorParameters = constructor.GetParameters();

                var moduleDependecies = new List<object>();
                foreach (var parameter in constructorParameters)
                {
                    var dependency = GetModule(parameter.ParameterType);
                    moduleDependecies.Add(dependency);
                }

                module = constructor.Invoke(moduleDependecies.ToArray());
            }

            return module;
        }

        private static object GetModule(Type interfaceType)
        {
            if (ResgisteredModules.ContainsKey(interfaceType))
            {
                var tupleModule = ResgisteredModules[interfaceType];
                return tupleModule.Item1 == Scope.Singletone
                    ? tupleModule.Item2
                    : getModuleInstance(tupleModule.Item2 as ConstructorInfo);
            }

            throw new Exception($"Module {interfaceType.FullName} isn't register");
        }
    }
}
