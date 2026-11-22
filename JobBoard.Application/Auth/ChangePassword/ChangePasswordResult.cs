namespace JobBoard.Application.Auth.ChangePassword
{
    public enum ChangePasswordOutcome
    {
        Changed,

        // The token is valid but its user no longer exists. The controller answers 401.
        UserNotFound,

        // Wrong current password, or a new password that breaks the rules. See Errors.
        Rejected
    }

    // The outcome of a password change, in the Application layer's own terms (no IdentityResult).
    public class ChangePasswordResult
    {
        public ChangePasswordOutcome Outcome { get; private init; }

        // Field name -> messages ("CurrentPassword", "NewPassword"), shaped like ModelState so the
        // controller can return a standard 400 ValidationProblem.
        public IReadOnlyDictionary<string, string[]> Errors { get; private init; } = new Dictionary<string, string[]>();

        public static ChangePasswordResult Changed() => new() { Outcome = ChangePasswordOutcome.Changed };

        public static ChangePasswordResult UserNotFound() => new() { Outcome = ChangePasswordOutcome.UserNotFound };

        public static ChangePasswordResult Rejected(IReadOnlyDictionary<string, string[]> errors) =>
            new() { Outcome = ChangePasswordOutcome.Rejected, Errors = errors };
    }
}
