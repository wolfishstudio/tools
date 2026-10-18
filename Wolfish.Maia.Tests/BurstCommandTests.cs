using System.Text.Json;
using Wolfish.Commands;
using Xunit;

namespace Wolfish.Maia.Tests
{
    public class BurstCommandTests
    {
        private readonly string _testJsonPath;

        public BurstCommandTests()
        {
            var baseDir = AppContext.BaseDirectory;
            _testJsonPath = Path.Combine(baseDir, "TestData", "test-burst-commands.json");
        }

        [Fact]
        public async Task SeekAndExecute_WithLessThan3Args_ReturnsFalse()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var args = new[] { "test" };

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SeekAndExecute_WithNonExistentFile_ReturnsFalse()
        {
            // Arrange
            var burst = new BurstCommand("nonexistent.json");
            var args = new[] { "test", "simple", "arg" };

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SeekAndExecute_WithNonMatchingVerb_ReturnsFalse()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var args = new[] { "nonexistent", "action", "arg" };

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SeekAndExecute_WithNonMatchingAction_ReturnsFalse()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var args = new[] { "test", "nonexistent", "arg" };

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SeekAndExecute_WithSimpleWildcard_ReturnsTrue()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var args = new[] { "test", "simple", "myarg" };

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SeekAndExecute_WithMultipleWildcards_ReturnsTrue()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var args = new[] { "test", "multi", "arg1", "arg2" };

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SeekAndExecute_WithWrongArgCount_ReturnsFalse()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var args = new[] { "test", "multi", "only-one-arg" }; // Expects 2 args

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SeekAndExecute_WithPrefixWildcard_MatchesCorrectly()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var validArgs = new[] { "test", "wildcard", "https://github.com", "repo.git" };
            var invalidArgs = new[] { "test", "wildcard", "ftp://server.com", "repo.git" };

            // Act
            var validResult = await burst.SeekAndExecute(validArgs);
            var invalidResult = await burst.SeekAndExecute(invalidArgs);

            // Assert
            Assert.True(validResult, "Should match http* prefix");
            Assert.False(invalidResult, "Should not match ftp:// for http* pattern");
        }

        [Fact]
        public async Task SeekAndExecute_WithSuffixWildcard_MatchesCorrectly()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var validArgs = new[] { "test", "wildcard", "https://example.com", "myrepo.git" };
            var invalidArgs = new[] { "test", "wildcard", "https://example.com", "myrepo.txt" };

            // Act
            var validResult = await burst.SeekAndExecute(validArgs);
            var invalidResult = await burst.SeekAndExecute(invalidArgs);

            // Assert
            Assert.True(validResult, "Should match *.git suffix");
            Assert.False(invalidResult, "Should not match .txt for *.git pattern");
        }

        [Fact]
        public async Task SeekAndExecute_WithLiteralMatch_RequiresExactValue()
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);
            var validArgs = new[] { "test", "literal", "-p", "8080" };
            var invalidArgs = new[] { "test", "literal", "--port", "8080" };

            // Act
            var validResult = await burst.SeekAndExecute(validArgs);
            var invalidResult = await burst.SeekAndExecute(invalidArgs);

            // Assert
            Assert.True(validResult, "Should match literal -p");
            Assert.False(invalidResult, "Should not match --port for -p pattern");
        }

        [Theory]
        [InlineData("test", "simple", "arg1")]
        [InlineData("test", "multi", "arg1", "arg2")]
        [InlineData("test", "wildcard", "http://test.com", "repo.git")]
        [InlineData("test", "wildcard", "https://test.com", "my-repo.git")]
        [InlineData("test", "literal", "-p", "3000")]
        public async Task SeekAndExecute_WithValidCombinations_ReturnsTrue(params string[] args)
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.True(result, $"Should match args: {string.Join(" ", args)}");
        }

        [Theory]
        [InlineData("test", "simple")] // Too few args
        [InlineData("test", "multi", "only-one")] // Wrong count
        [InlineData("test", "wildcard", "ftp://test.com", "repo.git")] // Wrong prefix
        [InlineData("test", "wildcard", "https://test.com", "repo.txt")] // Wrong suffix
        [InlineData("test", "literal", "--port", "3000")] // Wrong literal
        [InlineData("wrong", "simple", "arg")] // Wrong verb
        [InlineData("test", "wrong", "arg")] // Wrong action
        public async Task SeekAndExecute_WithInvalidCombinations_ReturnsFalse(params string[] args)
        {
            // Arrange
            var burst = new BurstCommand(_testJsonPath);

            // Act
            var result = await burst.SeekAndExecute(args);

            // Assert
            Assert.False(result, $"Should not match args: {string.Join(" ", args)}");
        }

        [Fact]
        public void BurstCommandDto_Serialization_WorksCorrectly()
        {
            // Arrange
            var dto = new BurstCommandDto
            {
                Verb = "git",
                Action = "clone",
                Args = new List<string> { "http*", "*" },
                Msg = "Test message",
                Stp = new List<BurstStep?>
                {
                    new BurstStep { Cmd = "git", Arg = "clone ${1} ${2}" }
                }
            };

            // Act
            var json = JsonSerializer.Serialize(dto);
            var deserialized = JsonSerializer.Deserialize<BurstCommandDto>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal("git", deserialized.Verb);
            Assert.Equal("clone", deserialized.Action);
            Assert.Equal(2, deserialized.Args?.Count);
            Assert.Equal("http*", deserialized.Args?[0]);
        }
    }
}
