using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IoC.CustomAttributes;

namespace IoC
{
    public class Container
    {
        private readonly IDictionary<Type, RegisteredObject> _registeredObjects = new Dictionary<Type, RegisteredObject>();

        public void AddAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(type => type.GetCustomAttributes<ExportAttribute>(false).SingleOrDefault() != null).ToArray();

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttributes<ExportAttribute>(false).SingleOrDefault();
                if (attribute != null)
                {
                    if (attribute.Type != null)
                        AddType(type, attribute.Type);

                    AddType(type);
                }      
            }        
        }

        public void AddType(Type type)
        {
            Register(type, type, false, null);
        }

        public void AddType(Type concrete, Type type)
        {
            Register(type, concrete, false, null);
        }

        public void AddTypeSingleton(Type type)
        {
            Register(type, type, true, type);
        }

        public void AddTypeSingleton(Type type, Type concrete)
        {
            Register(type, concrete, true, type);
        }

        public TType CreateInstance<TType>()
        {
            return (TType)ResolveObject(typeof(TType));
        }

        public object CreateInstance(Type type)
        {
            return ResolveObject(type);
        }

        private void Register(Type type, Type concrete, bool isSingleton, object instance)
        {
            if (_registeredObjects.ContainsKey(type))
                _registeredObjects.Remove(type);

            _registeredObjects.Add(type, new RegisteredObject(concrete, isSingleton, instance));
        }

        private object ResolveObject(Type type)
        {
            var registeredObject = _registeredObjects[type];
            if (registeredObject == null)
            {
                throw new ArgumentNullException($"The type {type.Name} has not been registered");
            }
            return GetInstance(registeredObject);
        }

        private object GetInstance(RegisteredObject registeredObject)
        {
            object instance = registeredObject.SingletonInstance;

            if (instance != null)
                return instance;

            var isConstructorAttribute = registeredObject.ConcreteType.GetCustomAttribute<ImportConstructorAttribute>(false) != null;

            if (isConstructorAttribute)
            {
                var parameters = ResolveConstructorParameters(registeredObject);
                instance = registeredObject.CreateInstance(parameters.ToArray());
            }
            else
            {
                instance = registeredObject.CreateInstance();
                ResolveProperty(registeredObject, ref instance);
            }
            return instance;
        }
      
        private IEnumerable<object> ResolveConstructorParameters(RegisteredObject registeredObject)
        {
            var constructorInfo = registeredObject.ConcreteType.GetConstructors().First();

            if(constructorInfo == null)
                throw new Exception($"There are no public constructors for type {registeredObject.ConcreteType.FullName}");

            return constructorInfo.GetParameters().Select(parameter => ResolveObject(parameter.ParameterType));
        }

        private void ResolveProperty(RegisteredObject registeredObject, ref object instance)
        {
            var properties = registeredObject.ConcreteType.GetProperties()
                .Where(x => x.CanWrite && x.IsDefined(typeof(ImportAttribute)));

            foreach (var property in properties)
            {
                property.SetValue(instance, ResolveObject(property.PropertyType));
            }
        }

        private class RegisteredObject
        {
            private readonly bool _isSinglton;

            public Type ConcreteType { get; private set; }
            public object SingletonInstance { get; private set; }

            public RegisteredObject(Type concreteType, bool isSingleton, object singletoninstance)
            {
                _isSinglton = isSingleton;
                SingletonInstance = singletoninstance;
                ConcreteType = concreteType;
            }

            public object CreateInstance(params object[] args)
            {
                object instance = Activator.CreateInstance(ConcreteType, args);

                if (_isSinglton)
                    SingletonInstance = instance;

                return instance;
            }
        }
    }
}
