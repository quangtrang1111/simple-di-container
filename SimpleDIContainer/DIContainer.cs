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
        private static readonly Dictionary<Type, (Scope, object)> _resgisteredModules = new Dictionary<Type, (Scope, object)>();

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
            if (_resgisteredModules.ContainsKey(interfaceType))
            {
                return;
            }

            if (!interfaceType.IsAssignableFrom(moduleType))
            {
                throw new Exception($"Module {interfaceType.FullName} wasn't implemented");
            }


            var firstConstructor = moduleType.GetConstructors().FirstOrDefault();
            _resgisteredModules.Add(interfaceType, (scope, firstConstructor));
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
            if (_resgisteredModules.ContainsKey(interfaceType))
            {
                var tupleModule = _resgisteredModules[interfaceType];

                switch (tupleModule.Item1)
                {
                    case Scope.Singletone:
                        if (tupleModule.Item2 is ConstructorInfo)
                        {
                            _resgisteredModules[interfaceType] = (tupleModule.Item1, getModuleInstance(tupleModule.Item2 as ConstructorInfo));
                            return _resgisteredModules[interfaceType].Item2;
                        }

                        return tupleModule.Item2;
                    case Scope.Transient:
                        return getModuleInstance(tupleModule.Item2 as ConstructorInfo);
                }


                
            }

            throw new Exception($"Module {interfaceType.FullName} isn't register");
        }
    }
}
