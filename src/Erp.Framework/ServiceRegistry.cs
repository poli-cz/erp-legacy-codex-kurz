using System;
using System.Collections.Generic;

namespace Erp.Framework
{
    /// <summary>
    /// Globální registr služeb (service locator). Každý modul se registruje ve své třídě *Module
    /// (PayrollModule.Register(), VatModule.Register()). Při startu aplikace volá registrace
    /// ErpApplication.Init() ve WinForms projektu, v testech je volá test sám.
    /// Pozor: stav je statický a sdílený celým procesem (i mezi testy).
    /// </summary>
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>();
        private static readonly object _lock = new object();

        public static void Register<T>(T instance) where T : class
        {
            Register<T>(() => instance);
        }

        public static void Register<T>(Func<T> factory) where T : class
        {
            lock (_lock)
            {
                _factories[typeof(T)] = () => factory();
            }
        }

        public static T Get<T>() where T : class
        {
            Func<object> factory;
            lock (_lock)
            {
                if (!_factories.TryGetValue(typeof(T), out factory))
                    throw new InvalidOperationException(
                        "Služba " + typeof(T).Name + " není registrována. Chybí volání *Module.Register()?");
            }
            return (T)factory();
        }

        public static bool IsRegistered<T>() where T : class
        {
            lock (_lock)
            {
                return _factories.ContainsKey(typeof(T));
            }
        }

        /// <summary>Jen pro testy.</summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _factories.Clear();
            }
        }
    }
}
