using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StudioMetrics.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {

        }

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<Artist> Artists { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual ICollection<Player> Players { get; set; }
    }
}
