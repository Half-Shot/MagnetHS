using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HalfShot.MagnetHS.DatastoreService.Records
{
    internal class UserRecord
    {
        [Key]
        public string UserId { get; set; }
        [Required]
        public bool IsLocal { get; set; }
        public DateTime CreationDt { get; set; }
        public DateTime UpdateDt { get; set; }
    }
}
