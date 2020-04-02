using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acrelec.Mockingbird.Payment_UK_IPP350
{
    public interface ICommunicatorCallbacks
    {
        void InitResponse(object parameters);

        void TestResponse(object parameters);

        void PayResponse(object parameters);

        void ProgressMessageResponse(object parameters);
    }
}
