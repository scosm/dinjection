using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class TransactionPeripheral : TransactionBase
    {
        public TransactionPeripheral(ILogger logger)
            : base(logger)
        {
        }

        public override bool Execute()
        {
            _logger.Info($"TRANSACTION (Peripheral). Order: {Order?.OrderDate}, {Order?.Product?.Name ?? ""}, {Order?.Product.Cost}");

            return true;
        }
    }
}
