using System;
using HalfShot.MagnetHS.RoomService.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HalfShot.MagnetHS.Tests.Services.RoomService
{
    public class ExpectUnauthorizedEvent : ExpectedExceptionBaseAttribute
    {
        public string Reason { get; set; }
        public ExpectUnauthorizedEvent(string reason)
        {
            Reason = reason;
        }
        
        protected override void Verify(Exception exception)
        {
            if (exception is GraphUnauthorizedException == false)
            {
                throw new AssertFailedException($"Exception was not of type GraphUnauthorizedException. Got {exception.GetType().Name}");
            }
            var ex = exception as GraphUnauthorizedException;
            if (ex.Reason != Reason)
            {
                throw new AssertFailedException($"Reason was not correct. Got {ex.Reason} instead of {Reason}");
            }
        }
    }
}