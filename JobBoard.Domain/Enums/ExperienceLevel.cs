namespace JobBoard.Domain.Enums
{
    // Seniority, shared by jobs (what is asked for) and candidates (what they have), so the two can be
    // compared directly when matching. Ordered from least to most senior; new values go at the end.
    public enum ExperienceLevel
    {
        Entry,
        Junior,
        Mid,
        Senior,
        Lead
    }
}
