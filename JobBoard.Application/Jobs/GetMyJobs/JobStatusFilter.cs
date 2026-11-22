namespace JobBoard.Application.Jobs.GetMyJobs
{
    // Which of the employer's own jobs GET /api/Job/mine?status=... returns.
    // Query binding is case-insensitive (status=open works); an unknown value is a 400.
    public enum JobStatusFilter
    {
        All,
        Open,
        Closed
    }
}
