using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrelec.Mockingbird.Payment_UK_IPP350
{
    public enum CommunicatorMethods : int
    {
        /// <summary>
        /// The name of the init method that is send/received in a pipe message
        /// </summary>
        Init = 0,

        /// <summary>
        /// The name of the test method that is send/received in a pipe message
        /// </summary>
        Test = 1,

        /// <summary>
        /// The name of the start payment procedure (print header, send items, print items, credit card payment)
        /// </summary>
        Pay = 2,

        /// <summary>
        /// The name of the progress message method that is send/received in a pipe message
        /// </summary>
        ProgressMessage = 4,
    }
}
