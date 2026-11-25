using JobBoard.Application.DTOs;

namespace JobBoard.Application.Jobs.ApplyToJob
{
    public enum ApplyToJobStatus
    {
        Created,
        JobNotFound,
        AlreadyApplied,
        DeadlinePassed
    }

    // The handler can end in four different ways; the controller maps each one to an HTTP status.
    public class ApplyToJobResult
    {
        public ApplyToJobStatus Status { get; private init; }
        public JobApplicationResponse? Application { get; private init; }

        public static ApplyToJobResult Success(JobApplicationResponse application) =>
            new() { Status = ApplyToJobStatus.Created, Application = application };

        public static ApplyToJobResult JobNotFound() =>
            new() { Status = ApplyToJobStatus.JobNotFound };

        public static ApplyToJobResult AlreadyApplied() =>
            new() { Status = ApplyToJobStatus.AlreadyApplied };

        public static ApplyToJobResult DeadlinePassed() =>
            new() { Status = ApplyToJobStatus.DeadlinePassed };
    }
}
