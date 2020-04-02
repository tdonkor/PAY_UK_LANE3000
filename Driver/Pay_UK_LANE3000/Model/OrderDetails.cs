using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAY_UK_IPP350.Model
{
    /// <summary>
    /// In a payment request details of the order will be provided 
    /// which will use this data model for the deserialization
    /// </summary>
    public class OrderDetails
    {
        /// <summary>
        /// Used to select the payment currency
        /// </summary>
        public string Currency { get; set; }
    }
}
