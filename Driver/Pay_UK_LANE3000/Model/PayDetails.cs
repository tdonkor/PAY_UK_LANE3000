using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAY_UK_IPP350.Model
{
    /// <summary>
    /// Object that contains the payment details
    /// </summary>
    public class PayDetails
    {
        public int PaidAmount { get; set; }
        public string TenderMediaId { get; set; }
        public string TenderMediaDetails { get; set; }
        public bool HasClientReceipt { get; set; }
        public bool HasMerchantReceipt { get; set; }
    }
}
