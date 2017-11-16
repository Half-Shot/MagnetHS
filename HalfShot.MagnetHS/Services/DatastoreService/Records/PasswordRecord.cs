using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HalfShot.MagnetHS.DatastoreService.Records
{
    internal class PasswordRecord
    {
        [Key]
        [Required]
        public string UserId { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Salt { get; set; }
        public DateTime CreationDt { get; set; }
        public DateTime UpdateDt { get; set; }
    }
}
