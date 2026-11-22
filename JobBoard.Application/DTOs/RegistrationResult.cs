namespace JobBoard.Application.DTOs
{
    // The outcome of a registration attempt, in the Application layer's own terms.
    // Replaces ASP.NET Core Identity's IdentityResult so the Application project does not depend on
    // Identity (a framework concern that belongs to Infrastructure).
    public class RegistrationResult
    {
        public bool Succeeded { get; private init; }

        // Field name -> messages, e.g. "Password" -> ["Passwords must have at least one digit."].
        // Shaped like ModelState so the controller can return it as a standard 400 ValidationProblem.
        public IReadOnlyDictionary<string, string[]> Errors { get; private init; } = new Dictionary<string, string[]>();

        public static RegistrationResult Success() => new() { Succeeded = true };

        public static RegistrationResult Failure(IReadOnlyDictionary<string, string[]> errors) =>
            new() { Succeeded = false, Errors = errors };
    }
}
