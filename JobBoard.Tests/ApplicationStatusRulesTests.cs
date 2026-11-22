using JobBoard.Domain.Enums;
using JobBoard.Domain.Rules;

namespace JobBoard.Tests
{
    // The hiring pipeline: Applied -> UnderReview -> Shortlisted -> Interview -> Offered -> Hired.
    public class ApplicationStatusRulesTests
    {
        [Theory]
        [InlineData(ApplicationStatus.Applied, ApplicationStatus.UnderReview)]
        [InlineData(ApplicationStatus.UnderReview, ApplicationStatus.Shortlisted)]
        [InlineData(ApplicationStatus.Shortlisted, ApplicationStatus.Interview)]
        [InlineData(ApplicationStatus.Interview, ApplicationStatus.Offered)]
        [InlineData(ApplicationStatus.Offered, ApplicationStatus.Hired)]
        public void Employer_can_move_one_step_forward(ApplicationStatus from, ApplicationStatus to)
        {
            Assert.True(ApplicationStatusRules.CanEmployerMove(from, to));
        }

        [Theory]
        [InlineData(ApplicationStatus.Applied, ApplicationStatus.Shortlisted)]
        [InlineData(ApplicationStatus.Applied, ApplicationStatus.Hired)]
        [InlineData(ApplicationStatus.UnderReview, ApplicationStatus.Offered)]
        public void Employer_cannot_skip_stages(ApplicationStatus from, ApplicationStatus to)
        {
            Assert.False(ApplicationStatusRules.CanEmployerMove(from, to));
        }

        [Theory]
        [InlineData(ApplicationStatus.UnderReview, ApplicationStatus.Applied)]
        [InlineData(ApplicationStatus.Offered, ApplicationStatus.Interview)]
        public void Employer_cannot_move_backwards(ApplicationStatus from, ApplicationStatus to)
        {
            Assert.False(ApplicationStatusRules.CanEmployerMove(from, to));
        }

        [Theory]
        [InlineData(ApplicationStatus.Applied)]
        [InlineData(ApplicationStatus.UnderReview)]
        [InlineData(ApplicationStatus.Shortlisted)]
        [InlineData(ApplicationStatus.Interview)]
        [InlineData(ApplicationStatus.Offered)]
        public void Employer_can_reject_from_any_open_stage(ApplicationStatus from)
        {
            Assert.True(ApplicationStatusRules.CanEmployerMove(from, ApplicationStatus.Rejected));
        }

        [Theory]
        [InlineData(ApplicationStatus.Hired)]
        [InlineData(ApplicationStatus.Rejected)]
        [InlineData(ApplicationStatus.Withdrawn)]
        public void Final_statuses_allow_no_employer_moves(ApplicationStatus final)
        {
            Assert.Empty(ApplicationStatusRules.EmployerNextStatuses(final));
        }

        [Fact]
        public void Employer_can_never_withdraw_on_behalf_of_the_applicant()
        {
            foreach (var from in Enum.GetValues<ApplicationStatus>())
            {
                Assert.False(ApplicationStatusRules.CanEmployerMove(from, ApplicationStatus.Withdrawn));
            }
        }

        [Theory]
        [InlineData(ApplicationStatus.Applied, true)]
        [InlineData(ApplicationStatus.Offered, true)]
        [InlineData(ApplicationStatus.Hired, false)]
        [InlineData(ApplicationStatus.Rejected, false)]
        [InlineData(ApplicationStatus.Withdrawn, false)]
        public void Applicant_can_withdraw_only_before_the_process_ends(ApplicationStatus from, bool expected)
        {
            Assert.Equal(expected, ApplicationStatusRules.CanWithdraw(from));
        }
    }
}
