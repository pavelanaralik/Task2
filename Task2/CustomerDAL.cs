using System;
using System.Collections.Generic;
using System.Text;
using IoC.CustomAttributes;

namespace Task2
{
    [Export(typeof(ICustomerDAL))]
    public class CustomerDAL: ICustomerDAL
    {
    }
}
