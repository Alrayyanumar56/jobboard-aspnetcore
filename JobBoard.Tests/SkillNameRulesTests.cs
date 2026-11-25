using JobBoard.Domain.Rules;

namespace JobBoard.Tests
{
    public class SkillNameRulesTests
    {
        [Theory]
        [InlineData("  ASP.NET   Core ", "ASP.NET Core")]
        [InlineData("C#", "C#")]
        [InlineData("\tSQL\n Server", "SQL Server")]
        public void Clean_trims_and_collapses_whitespace(string raw, string expected)
        {
            Assert.Equal(expected, SkillNameRules.Clean(raw));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Clean_returns_null_for_blank(string? raw)
        {
            Assert.Null(SkillNameRules.Clean(raw));
        }

        [Fact]
        public void Different_casing_normalizes_to_the_same_key()
        {
            Assert.Equal(SkillNameRules.Normalize("asp.net core"), SkillNameRules.Normalize("ASP.NET Core"));
        }

        [Fact]
        public void CleanList_keeps_first_spelling_drops_blanks_and_repeats_in_order()
        {
            var result = SkillNameRules.CleanList(["React", " c# ", "", "REACT", "C#", null, "SQL"]);

            Assert.Equal(["React", "c#", "SQL"], result);
        }

        [Fact]
        public void CleanList_of_null_is_empty()
        {
            Assert.Empty(SkillNameRules.CleanList(null));
        }
    }
}
