using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _connection;

        public DatabaseLogger(string connection)
        {
            _connection = connection;
            // Open connection to database receiving log messages....
        }

        public void Info(string message)
        {
            Console.WriteLine($"INFO (to database): {message}");
        }


        public void Error(string message)
        {
            Console.WriteLine($"*** ERROR (to database): {message}");
        }
    }
}
