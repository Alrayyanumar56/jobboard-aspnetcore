using System.Text.Json.Serialization;

namespace JobBoard.Domain.Enums
{
    // Where a JobApplication is in the hiring process. Values are stored in the database as numbers,
    // so they are fixed explicitly: never reorder or renumber them, only add new ones at the end.
    // Serialized as text ("UnderReview", not 1) so Swagger requests and API responses are readable.
    // This only affects this enum; JobType and Location keep their current number format.
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApplicationStatus
    {
        Applied = 0,
        UnderReview = 1,
        Shortlisted = 2,
        Interview = 3,
        Offered = 4,
        Hired = 5,
        Rejected = 6,
        Withdrawn = 7
    }
}
