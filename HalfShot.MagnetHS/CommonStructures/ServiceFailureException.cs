using System;
using HalfShot.MagnetHS.CommonStructures.Responses;

namespace HalfShot.MagnetHS.CommonStructures
{
    public class ServiceFailureException : Exception
    {
        public StatusResponse Response { get; private set; }
        
        public ServiceFailureException(string message) : base(message)
        {
            
        }
        
        public ServiceFailureException(StatusResponse response) : base(response.Error)
        {
            Response = response;
        }
    }
}