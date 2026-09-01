using JobBoard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JobBoard.Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Title should be under 100 characters")]
        public String Title { get; set; }
        [Required]
        [MaxLength(1000,ErrorMessage ="Description should be under 1000 characters")]
        public String Description { get; set; }
        public JobType JobType { get; set; }
        public Location JobLocation { get; set; }
        [Required]
        public String EmployerId { get; set; }
        [Required]
        public DateTimeOffset PostedAt{ get; set; }
        public String? SalaryRange { get; set; }
    }
}
