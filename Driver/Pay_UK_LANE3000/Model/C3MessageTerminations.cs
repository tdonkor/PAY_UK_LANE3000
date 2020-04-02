using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAY_UK_IPP350.Model
{
    public static class C3MessageTerminations
    {
        /// <summary>
        /// End of data marker (Data Link Escape). This field must contain the value 10h
        /// </summary>
        public const char DLE = (char)0x10;

        /// <summary>
        /// End of message marker (End of Text). This field must contain the value 03h
        /// </summary>
        public const char ETX = (char)0x03;
    }
}
