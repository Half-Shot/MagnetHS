using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HalfShot.MagnetHS.DatastoreService.Records
{
    internal class EventRecord
    {
        [Key]
        public string EventId { get; set; }
        public string Sender { get; set; }
        public string RoomId { get; set; }
        public DateTime OriginServerTs { get; set; }
        public int Depth { get; set; }
        public string StateKey { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public string UnsignedContent { get; set; }
        public string Origin { get; set; }
    }
}
