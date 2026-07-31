using System.ComponentModel.DataAnnotations;

namespace Job_Board.Model
{
    public class job
    {
        public int jobID { set; get; }
        [Required]
        [MaxLength (100,ErrorMessage ="Title Should be less than 100 characters")]
        public string jobTitle { set; get; }
        [Required]
        [MaxLength (100,ErrorMessage ="Description Should be less than 5000 characters")]
        public string jobDescription { set; get; }
        [Required]
        public bool activeStatus { set; get; }
        [Required]
        public jobType? jobType { set; get; }
        [Required]
        public DateTimeOffset openingDate { set; get; }




    };
}
