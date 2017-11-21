using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HalfShot.MagnetHS.DatastoreService.Records
{
    internal class RoomRecord
    {
        [Key]
        public string RoomId { get; set; }
    }
}
