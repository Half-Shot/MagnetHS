using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
namespace HalfShot.MagnetHS.RoomService.Exceptions
{
    public class GraphInsertionException : Exception
    {
        PDUEvent ev;
        public GraphInsertionException(PDUEvent context_ev, RoomGraph graph) : base ($"Could not insert {context_ev.EventId} into {context_ev.RoomId}") {
            Data.Add("PDUEvent",context_ev);
            Data.Add("RoomGraph", graph);
        }

        public GraphInsertionException(string message, PDUEvent context_ev, RoomGraph graph) : base (message) {
            Data.Add("PDUEvent",context_ev);
            Data.Add("RoomGraph", graph);
        }
    }

    public class GraphInvalidRoomIdException : GraphInsertionException
    {
        public GraphInvalidRoomIdException(RoomGraph graph, PDUEvent context_ev) : base (
$"Bad RoomID for {context_ev.EventId}. Tried to insert into {graph.RoomId}, but was meant for {context_ev.RoomId}", context_ev, graph) {
        }
    }
    
    public class GraphInvalidSenderException : GraphInsertionException
    {
        public GraphInvalidSenderException(RoomGraph graph, PDUEvent context_ev) : base (
            $"Bad sender for {context_ev.EventId}. Sender is '{context_ev.Sender}'", context_ev, graph) {
        }
    }
    
    public class GraphInvalidTypeException : GraphInsertionException
    {
        public GraphInvalidTypeException(RoomGraph graph, PDUEvent context_ev) : base (
            $"Bad type for {context_ev.EventId}. Type is '{context_ev.Type}'", context_ev, graph) {
        }
    }
    
    public class GraphInvalidEventIdException : GraphInsertionException
    {
        public GraphInvalidEventIdException(RoomGraph graph, PDUEvent context_ev) : base (
            $"Bad eventId for {context_ev.EventId}.", context_ev, graph) {
        }
    }
    
    public class GraphUnauthorizedException : GraphInsertionException
    {
        public string Reason { get; set; }
        public GraphUnauthorizedException(RoomGraph graph, PDUEvent context_ev, string reason) : base (
            $"{context_ev.EventId} could not be authorized for {graph.RoomId}. Reason: {reason}", context_ev, graph)
        {
            Reason = reason;
            Data.Add("Reason", reason);
        }
    }
}