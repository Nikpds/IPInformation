using System;
using System.Collections.Generic;
using System.Text;

namespace IpInfoProvider
{
    public interface IIPInfoProvider
    {
        IPDetails GetDetails(string ip);
    }
}
