using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Exceptions
{
    class RestContentTypeNotSupported : RestError
    {
        public RestContentTypeNotSupported() : base()
        {

        }

        public RestContentTypeNotSupported(string message) : base(message)
        {

        }
    }
}
