using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
    public class TransactionComputer : TransactionBase
    {
        public TransactionComputer(ILogger logger)
            : base(logger)
        {
        }

        public override bool Execute()
        {
            _logger.Info($"TRANSACTION (Computer). Order: {Order?.OrderDate}, {Order?.Product?.Name ?? ""}, {Order?.Product.Cost}");

            return true;
        }
    }
}
