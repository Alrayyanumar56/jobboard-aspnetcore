using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Job_Board.Model
{
    public class Job
    {
        public int jobID { set; get; }
        [Required]
        [MaxLength (100,ErrorMessage ="Title Should be less than 100 characters")]
        public string Title { set; get; }
        [Required]
        [MaxLength (5000,ErrorMessage ="Description Should be less than 5000 characters")]
        public string Description { set; get; }
        [Required]
        public string Location { set; get; }
        public string? SalaryRange { set; get; }
        [Required]
        public jobType? JobType { set; get; }
        [Required]
        public DateTimeOffset PostingDate { set; get; }
        public string EmployerID { set; get; }
        
    };
}
