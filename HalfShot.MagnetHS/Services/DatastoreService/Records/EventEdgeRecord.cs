using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HalfShot.MagnetHS.DatastoreService.Records
{
    internal class EventEdgeRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PrevEventId { get; set; }
        public string EventId { get; set; }
    }
}
