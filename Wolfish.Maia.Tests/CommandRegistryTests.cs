using Wolfish.Maia.Commands;
using Xunit;

namespace Wolfish.Maia.Tests
{
    public class CommandRegistryTests
    {
        private class TestCommand : ICliCommand
        {
            public string Name => "testcmd";
            public bool WasExecuted { get; private set; }

            public Task ExecuteAsync(string[] args)
            {
                WasExecuted = true;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void CreateDefault_ContainsStandardCommands()
        {
            // Arrange & Act
            var registry = CommandRegistry.CreateDefault();

            // Assert - Should have registered commands
            Assert.NotNull(registry);
        }

        [Fact]
        public async Task Register_AddsCommandToRegistry()
        {
            // Arrange
            var registry = new CommandRegistry();
            var testCommand = new TestCommand();

            // Act
            registry.Register(testCommand);
            var found = await registry.TryExecuteAsync("testcmd", new[] { "testcmd" });

            // Assert
            Assert.True(found);
            Assert.True(testCommand.WasExecuted);
        }

        [Fact]
        public async Task TryExecuteAsync_WithUnknownCommand_ReturnsFalse()
        {
            // Arrange
            var registry = new CommandRegistry();

            // Act
            var result = await registry.TryExecuteAsync("unknown", new[] { "unknown" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task TryExecuteAsync_IsCaseInsensitive()
        {
            // Arrange
            var registry = new CommandRegistry();
            var testCommand = new TestCommand();
            registry.Register(testCommand);

            // Act
            var result1 = await registry.TryExecuteAsync("testcmd", new[] { "testcmd" });
            var result2 = await registry.TryExecuteAsync("TESTCMD", new[] { "TESTCMD" });
            var result3 = await registry.TryExecuteAsync("TestCmd", new[] { "TestCmd" });

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
        }

        [Fact]
        public async Task Register_ReturnsRegistryForChaining()
        {
            // Arrange
            var registry = new CommandRegistry();
            var cmd1 = new TestCommand();

            // Act - Should allow chaining
            var result = registry.Register(cmd1);

            // Assert
            Assert.Same(registry, result);
        }

        [Fact]
        public async Task DefaultRegistry_ContainsExpectedCommands()
        {
            // Arrange
            var registry = CommandRegistry.CreateDefault();

            // Act & Assert - Test that standard commands are registered
            var helpFound = await registry.TryExecuteAsync("help", new[] { "help" });
            var welcomeFound = await registry.TryExecuteAsync("welcome", new[] { "welcome" });
            var providerFound = await registry.TryExecuteAsync("provider", new[] { "provider" });

            Assert.True(helpFound, "help command should be registered");
            Assert.True(welcomeFound, "welcome command should be registered");
            Assert.True(providerFound, "provider command should be registered");
        }
    }
}
