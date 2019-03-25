using IoC;
using System;
using System.Collections.Generic;
using System.Text;
using IoC.CustomAttributes;

namespace Task2
{
    //[ImportConstructor]
    [Export]
    public class CustomerBLL
    {
        //[Import]
        public ICustomerDAL CustomerDAL { get; set; }

        //[Import]
        public Logger Logger { get; set; }

        public string Name { get; set; }

        public CustomerBLL()
        {
        }

        public CustomerBLL(ICustomerDAL dal, Logger logger)
        {
            CustomerDAL = dal;
            Logger = logger;
        }

        public void Test()
        {
            Console.WriteLine("CustomerBLL.Test");
            var isCustomerDAL = this.CustomerDAL != null;
            var isLogger = this.Logger != null;
            Console.WriteLine(isCustomerDAL);
            Console.WriteLine(isLogger);
        }
    }
}
