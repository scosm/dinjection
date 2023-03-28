using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class TextLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"INFO: {message}");
        }


        public void Error(string message)
        {
            Console.WriteLine($"*** ERROR: {message}");
        }
    }
}
