using JobBoard.Domain.Enums;

namespace JobBoard.Domain.Rules
{
    // The one place that says which status changes are allowed. Handlers ask this class;
    // they never decide transitions themselves.
    //
    //   Applied -> UnderReview -> Shortlisted -> Interview -> Offered -> Hired
    //
    // The employer moves an application one step forward along that line, or rejects it from any
    // step before Hired. Stages can't be skipped or reversed. The JobSeeker can only withdraw.
    // Hired, Rejected and Withdrawn are final: nothing can leave them.
    public static class ApplicationStatusRules
    {
        // Employer moves only. Withdrawn is deliberately absent: only the JobSeeker can withdraw.
        private static readonly Dictionary<ApplicationStatus, ApplicationStatus[]> EmployerMoves = new()
        {
            [ApplicationStatus.Applied] = [ApplicationStatus.UnderReview, ApplicationStatus.Rejected],
            [ApplicationStatus.UnderReview] = [ApplicationStatus.Shortlisted, ApplicationStatus.Rejected],
            [ApplicationStatus.Shortlisted] = [ApplicationStatus.Interview, ApplicationStatus.Rejected],
            [ApplicationStatus.Interview] = [ApplicationStatus.Offered, ApplicationStatus.Rejected],
            [ApplicationStatus.Offered] = [ApplicationStatus.Hired, ApplicationStatus.Rejected]
        };

        // A JobSeeker may withdraw at any point before the process ends (including declining an offer).
        private static readonly ApplicationStatus[] WithdrawableFrom =
        [
            ApplicationStatus.Applied,
            ApplicationStatus.UnderReview,
            ApplicationStatus.Shortlisted,
            ApplicationStatus.Interview,
            ApplicationStatus.Offered
        ];

        public static IReadOnlyList<ApplicationStatus> EmployerNextStatuses(ApplicationStatus from) =>
            EmployerMoves.TryGetValue(from, out var next) ? next : Array.Empty<ApplicationStatus>();

        public static bool CanEmployerMove(ApplicationStatus from, ApplicationStatus to) =>
            EmployerNextStatuses(from).Contains(to);

        public static bool CanWithdraw(ApplicationStatus from) =>
            WithdrawableFrom.Contains(from);
    }
}
