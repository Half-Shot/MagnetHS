using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
namespace HalfShot.MagnetHS.RoomService
{
    class GraphInsertionException : Exception
    {
        PDUEvent ev;
        public GraphInsertionException(string message) : base (message) {

        }

        public GraphInsertionException(string message, PDUEvent context_ev) : base (message) {
        
        }
    }
}