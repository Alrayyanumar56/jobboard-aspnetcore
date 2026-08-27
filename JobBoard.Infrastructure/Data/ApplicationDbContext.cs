using System;
using System.Collections.Generic;
using System.Text;
using JobBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Data
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<Job> Job
            { get; set; }
    }
}
