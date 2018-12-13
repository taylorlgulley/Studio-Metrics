using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudioMetrics.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(55, ErrorMessage = "Please shorten the Project title to 55 characters")]
        public string Title { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a Project Type")]
        public int ProjectTypeId { get; set; }

        public ProjectType ProjectType { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(1, double.MaxValue, ErrorMessage = "Please choose a Payrate")]
        public double Payrate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a legnth of time in days")]
        [Display(Name = "Time Table")]
        public int TimeTable { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a Status")]
        [Display(Name = "Status")]
        public int StatusTypeId { get; set; }

        public StatusType StatusType { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a Client")]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        public Client Client { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public virtual ICollection<PlayerProject> PlayerProjects { get; set; }

        public virtual ICollection<ArtistProject> ArtistProjects { get; set; }
    }
}


