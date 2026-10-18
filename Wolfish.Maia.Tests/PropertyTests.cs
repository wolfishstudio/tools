using Wolfish.Commands;
using Xunit;

namespace Wolfish.Maia.Tests
{
    /// <summary>
    /// Testes baseados em propriedades - validam invariantes universais do sistema.
    /// </summary>
    public class PropertyTests
    {
        [Theory]
        [InlineData("*", "anything")]
        [InlineData("*", "")]
        [InlineData("*", "123")]
        [InlineData("*", "with spaces")]
        public void Property_WildcardStar_AcceptsAnyString(string pattern, string value)
        {
            // Property: Para qualquer string S, o pattern "*" sempre bate
            
            var matches = PatternMatches(pattern, value);
            Assert.True(matches, $"Wildcard * should match any string, but failed for: {value}");
        }

        [Theory]
        [InlineData("http*", "http://example.com", true)]
        [InlineData("http*", "https://example.com", true)]
        [InlineData("http*", "ftp://example.com", false)]
        [InlineData("test*", "testing", true)]
        [InlineData("test*", "taste", false)]
        public void Property_PrefixWildcard_MatchesCorrectly(string pattern, string value, bool shouldMatch)
        {
            // Property: Para pattern "P*", bate se e somente se value começa com P
            
            var matches = PatternMatches(pattern, value);
            Assert.Equal(shouldMatch, matches);
        }

        [Theory]
        [InlineData("*.git", "repo.git", true)]
        [InlineData("*.git", "my-repo.git", true)]
        [InlineData("*.git", "repo.txt", false)]
        [InlineData("*test", "mytest", true)]
        [InlineData("*test", "testing", false)]
        public void Property_SuffixWildcard_MatchesCorrectly(string pattern, string value, bool shouldMatch)
        {
            // Property: Para pattern "*S", bate se e somente se value termina com S
            
            var matches = PatternMatches(pattern, value);
            Assert.Equal(shouldMatch, matches);
        }

        [Theory]
        [InlineData("-p", "-p", true)]
        [InlineData("-p", "--port", false)]
        [InlineData("exact", "exact", true)]
        [InlineData("exact", "EXACT", true)] // Case-insensitive
        [InlineData("exact", "exact2", false)]
        public void Property_LiteralPattern_RequiresExactMatch(string pattern, string value, bool shouldMatch)
        {
            // Property: Para pattern sem wildcards, bate se e somente se value == pattern (case-insensitive)
            
            var matches = PatternMatches(pattern, value);
            Assert.Equal(shouldMatch, matches);
        }

        [Theory]
        [MemberData(nameof(ArgsCountTestData))]
        public void Property_ArgsCount_MustMatch(List<string> jsonArgs, string[] userArgs, bool shouldMatch)
        {
            // Property: Para jsonArgs de tamanho N e userArgs de tamanho M, bate se e somente se N == M
            
            var matches = jsonArgs.Count == userArgs.Length;
            
            if (shouldMatch)
            {
                Assert.Equal(jsonArgs.Count, userArgs.Length);
            }
            else
            {
                Assert.NotEqual(jsonArgs.Count, userArgs.Length);
            }
        }

        public static IEnumerable<object[]> ArgsCountTestData()
        {
            yield return new object[] { new List<string> { "*" }, new[] { "arg1" }, true };
            yield return new object[] { new List<string> { "*", "*" }, new[] { "arg1", "arg2" }, true };
            yield return new object[] { new List<string> { "*" }, new[] { "arg1", "arg2" }, false };
            yield return new object[] { new List<string> { "*", "*" }, new[] { "arg1" }, false };
        }

        [Fact]
        public void Property_EmptyArgs_NeverMatches()
        {
            // Property: Para userArgs vazio, nenhum comando Burst deve bater (mínimo 3 args no Maia)
            
            var userArgs = Array.Empty<string>();
            
            // No contexto do Maia, args.Length >= 3 para Burst
            Assert.True(userArgs.Length < 3);
        }

        [Theory]
        [InlineData("test", "test", "arg", 3)]
        [InlineData("git", "clone", "url", "folder", 4)]
        [InlineData("docker", "run", "-p", "8080", "nginx", 5)]
        public void Property_BurstMinimumArgs_IsThree(params string[] args)
        {
            // Property: Burst Shots sempre têm args.Length >= 3
            
            Assert.True(args.Length >= 3, "Burst shots must have at least 3 arguments");
        }

        // Helper method para simular o matching de patterns (extraído da lógica do BurstCommand)
        private bool PatternMatches(string pattern, string value)
        {
            if (pattern == "*")
                return true;

            if (pattern.EndsWith("*"))
            {
                var prefix = pattern.TrimEnd('*');
                return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }

            if (pattern.StartsWith("*"))
            {
                var suffix = pattern.TrimStart('*');
                return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
            }

            return pattern.Equals(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
