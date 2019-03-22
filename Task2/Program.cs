using IoC;
using System;
using System.Reflection;

namespace Task2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //Assembly currentAssem = Assembly.GetExecutingAssembly();

            var container = new Container();
            container.AddAssembly(Assembly.GetExecutingAssembly());
            //container.AddType(typeof(CustomerBLL));
            //container.AddType(typeof(Logger));
            //container.AddType(typeof(ICustomerDAL), typeof(CustomerDAL));
            //var customerBLL = container.CreateInstance<CustomerBLL>();
            var customerBLL = (CustomerBLL)container.CreateInstance(
                typeof(CustomerBLL));

            customerBLL.Test();
        }
    }
}
