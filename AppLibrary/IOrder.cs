using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public interface IOrder
    {
        string Id { get; set; }

        IProduct Product { get; set; }

        DateTime OrderDate { get; set; }


    }
}
