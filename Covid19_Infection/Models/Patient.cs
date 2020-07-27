using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Covid19_Infection.Models
{
    public class Patient
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Name is missing")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Range(0,120,ErrorMessage = "Invalid age")]
        [Required(ErrorMessage = "Age is missing")]
        public int Age { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Do not enter more than 100 characters")]
        public string  Address { get; set; }

        [Required(ErrorMessage = "Name is missing")]
        public int RelatedId { get; set; }

        [Required(ErrorMessage = "Name is missing")]
        public StateType State { get; set; }

        [Required(ErrorMessage = "Name is missing")]
        public CriticalityType Criticality { get; set; }
        
        public DateTime IdentifiedOn { get; set; }

        public enum StateType
        {
            Infected,
            Recovered,
            Deceased
        }
        public enum CriticalityType
        {
            Low,
            Medium,
            High
        }
    }
}