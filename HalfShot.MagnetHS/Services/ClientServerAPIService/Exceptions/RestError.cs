using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Exceptions
{
    class RestError : Exception
    {
        public string ErrorCode { get; set; } = "M_UNKNOWN";
        public int StatusCode { get; set; } = 500;
        public RestError() : base()
        {

        }

        public RestError(string message) : base(message)
        {

        }

        public RestError(string message, string errorcode) : base(message)
        {
            this.ErrorCode = errorcode;
        }

        public RestError(string message, Exception ex) : base(message, ex)
        {

        }

        public RestError(string message, string errorcode, Exception ex) : base(message, ex)
        {
            this.ErrorCode = errorcode;
        }

    }
}
