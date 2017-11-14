using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Exceptions
{
    class RestBadParameters : RestError
    {
        public RestBadParameters(string parametername) : this(parametername, null)
        {

        }

        public RestBadParameters(string parametername, Exception ex) : base($"Bad parameter given for request. Parameter: '{parametername}'", ex)
        {

        }
    }
}
