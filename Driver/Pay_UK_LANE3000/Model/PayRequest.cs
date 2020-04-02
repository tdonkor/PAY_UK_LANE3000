using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAY_UK_IPP350.Model
{
    /// <summary>
    /// Class that will be used to deserialize a json string given as a parameter to the "Pay" method
    /// </summary>
    public class PayRequest
    {
        /// <summary>
        /// The Total of the payment
        /// </summary>
        public int Amount { get; set; }

        public string TransactionReference { get; set; }
    }
}
