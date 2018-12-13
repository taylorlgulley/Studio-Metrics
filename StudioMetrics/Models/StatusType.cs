﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models
{
    public class StatusType
    {
        [Key]
        public int StatusTypeId { get; set; }

        [Required]
        public string Type { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}
