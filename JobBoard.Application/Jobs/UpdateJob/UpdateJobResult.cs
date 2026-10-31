using JobBoard.Application.DTOs;

namespace JobBoard.Application.Jobs.UpdateJob
{
    public enum UpdateJobStatus
    {
        Updated,
        JobNotFound,
        JobClosed
    }

    // The handler can end in three different ways; the controller maps each one to an HTTP status.
    public class UpdateJobResult
    {
        public UpdateJobStatus Status { get; private init; }
        public JobResponse? Job { get; private init; }

        public static UpdateJobResult Success(JobResponse job) =>
            new() { Status = UpdateJobStatus.Updated, Job = job };

        // Covers both "no such job" and "someone else's job" on purpose.
        public static UpdateJobResult JobNotFound() =>
            new() { Status = UpdateJobStatus.JobNotFound };

        public static UpdateJobResult JobClosed() =>
            new() { Status = UpdateJobStatus.JobClosed };
    }
}
