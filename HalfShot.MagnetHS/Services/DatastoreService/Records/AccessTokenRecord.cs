using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HalfShot.MagnetHS.DatastoreService.Records
{
    internal class AccessTokenRecord
    {
        [Key]
        public string AccessToken { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime CreationDt { get; set; }
        public string DeviceId { get; set; }
        public DateTime ExpiryDt { get; set; }
    }
}