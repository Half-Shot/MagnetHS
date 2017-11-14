using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HalfShot.MagnetHS.DatastoreService.Records
{
    internal class ProfileRecord
    {
        [Key]
        [Required]
        public string UserId { get; set; }
        [Key]
        [Required]
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreationDt { get; set; }
        public DateTime UpdateDt { get; set; }
    }
}
