using Wolfish.Maia.Commands;
using Xunit;

namespace Wolfish.Maia.Tests
{
    public class ProviderCommandTests
    {
        [Fact]
        public void ProviderCommand_Name_IsProvider()
        {
            // Arrange
            var command = new ProviderCommand();

            // Act
            var name = command.Name;

            // Assert
            Assert.Equal("provider", name);
        }

        [Fact]
        public async Task ExecuteAsync_WithNoSubcommand_ShowsHelp()
        {
            // Arrange
            var command = new ProviderCommand();
            var args = new[] { "provider" };
            
            // Capture console output
            var originalOutput = Console.Out;
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            await command.ExecuteAsync(args);

            // Restore console
            Console.SetOut(originalOutput);
            var output = writer.ToString();

            // Assert
            Assert.Contains("Gerenciamento de LLM Providers", output);
            Assert.Contains("list", output);
            Assert.Contains("add", output);
            Assert.Contains("edit", output);
        }

        [Fact]
        public async Task ExecuteAsync_WithInvalidSubcommand_ShowsError()
        {
            // Arrange
            var command = new ProviderCommand();
            var args = new[] { "provider", "invalid" };
            
            var originalOutput = Console.Out;
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            await command.ExecuteAsync(args);

            Console.SetOut(originalOutput);
            var output = writer.ToString();

            // Assert
            Assert.Contains("Subcomando desconhecido", output);
        }

        [Theory]
        [InlineData("list")]
        [InlineData("LIST")]
        [InlineData("List")]
        public async Task ExecuteAsync_SubcommandIsCaseInsensitive(string subcommand)
        {
            // Arrange
            var command = new ProviderCommand();
            var args = new[] { "provider", subcommand };
            
            var originalOutput = Console.Out;
            using var writer = new StringWriter();
            Console.SetOut(writer);

            // Act
            await command.ExecuteAsync(args);

            Console.SetOut(originalOutput);
            var output = writer.ToString();

            // Assert - Should not show error for valid subcommands
            Assert.DoesNotContain("Subcomando desconhecido", output);
        }
    }
}
