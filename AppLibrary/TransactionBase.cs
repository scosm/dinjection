using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class TransactionBase : ITransaction
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public virtual IOrder Order { get; set; }

        public virtual ILogger _logger { get; set; }


        public TransactionBase(ILogger logger)
        {
            _logger = logger;
        }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public virtual bool Execute()
        {
            _logger.Info($"TRANSACTION (BASE). Order: {Order?.OrderDate}, {Order?.Product?.Name ?? ""}, {Order?.Product.Cost}");

            return true;
        }
    }
}
