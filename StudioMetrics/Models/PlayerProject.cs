using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models
{
    public class PlayerProject
    {
        [Key]
        public int PlayerProjectId { get; set; }

        [Required]
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; }
    }
}
