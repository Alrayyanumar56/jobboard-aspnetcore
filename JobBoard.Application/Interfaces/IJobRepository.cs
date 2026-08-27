using System;
using System.Collections.Generic;
using System.Text;
using JobBoard.Domain.Entities;

namespace JobBoard.Application.Interfaces
{
    public interface IJobRepository
    {
        public Job AddJob(Job job);
    }
}
