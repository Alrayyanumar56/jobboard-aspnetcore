using JobBoard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobBoard.Application.DTOs
{
    public class JobResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public JobType JobType { get; set; }
        public Location JobLocation { get; set; }
        public string? SalaryRange { get; set; }
        public string EmployerId { get; set; } = string.Empty;
        public DateTimeOffset PostedAt { get; set; }
    }
}
