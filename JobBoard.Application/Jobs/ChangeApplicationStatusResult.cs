using JobBoard.Application.DTOs;

namespace JobBoard.Application.Jobs
{
    public enum ChangeApplicationStatusOutcome
    {
        Changed,
        NotFound,
        InvalidTransition
    }

    // Shared by the employer status change and the JobSeeker withdrawal: both can end in the same
    // three ways, and the controller maps each one to an HTTP status.
    public class ChangeApplicationStatusResult
    {
        public ChangeApplicationStatusOutcome Outcome { get; private init; }
        public ApplicationStatusResponse? Application { get; private init; }

        // Only set for InvalidTransition: a human-readable reason for the 409.
        public string? Message { get; private init; }

        public static ChangeApplicationStatusResult Success(ApplicationStatusResponse application) =>
            new() { Outcome = ChangeApplicationStatusOutcome.Changed, Application = application };

        // Covers both "no such application" and "not yours" on purpose.
        public static ChangeApplicationStatusResult NotFound() =>
            new() { Outcome = ChangeApplicationStatusOutcome.NotFound };

        public static ChangeApplicationStatusResult InvalidTransition(string message) =>
            new() { Outcome = ChangeApplicationStatusOutcome.InvalidTransition, Message = message };
    }
}
