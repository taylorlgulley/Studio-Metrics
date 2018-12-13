using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models
{
    public class ClientArtist
    {
        [Key]
        public int ClientArtistId { get; set; }

        [Required]
        public int ClientId { get; set; }

        public Client Client { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public Artist Artist { get; set; }
    }
}
