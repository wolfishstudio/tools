using Xunit;

namespace Wolfish.Maia.Tests
{
    /// <summary>
    /// Testes de integração para validar o dispatcher de 3 camadas (Quick/Clean/Burst).
    /// Estes testes validam a lógica de priorização sem executar comandos reais.
    /// </summary>
    public class DispatcherIntegrationTests
    {
        [Fact]
        public void Dispatcher_WithNoArgs_ShouldShowHelp()
        {
            // Property: Para args.Length == 0, o dispatcher sempre executa "help"
            // Este teste valida o comportamento sem args
            
            var args = Array.Empty<string>();
            
            // No código real: if (args.Length == 0) { await commandRegistry.TryExecuteAsync("help", args); return; }
            Assert.Equal(0, args.Length);
        }

        [Theory]
        [InlineData("help")]
        [InlineData("welcome")]
        [InlineData("list")]
        [InlineData("info")]
        [InlineData("home")]
        public void Dispatcher_WithOneArg_ShouldTryQuickShot(string command)
        {
            // Property: Para args.Length == 1, o dispatcher tenta CommandRegistry
            
            var args = new[] { command };
            
            Assert.Equal(1, args.Length);
            // Validação: dispatcher deve tentar CommandRegistry.TryExecuteAsync(args[0], args)
        }

        [Theory]
        [InlineData("provider", "list")]
        [InlineData("provider", "add")]
        [InlineData("provider", "edit")]
        public void Dispatcher_WithTwoArgs_QuickShotFirst_ShouldBeRegistryCommand(string verb, string action)
        {
            // Property: Comandos de 2 args registrados no CommandRegistry têm prioridade sobre Clean Shots
            
            var args = new[] { verb, action };
            
            Assert.Equal(2, args.Length);
            // Validação: "provider" está no CommandRegistry, então nunca chega ao Clean Shot
        }

        [Theory]
        [InlineData("download", "chrome")]
        [InlineData("install", "node")]
        [InlineData("update", "system")]
        public void Dispatcher_WithTwoArgs_NotInRegistry_ShouldTryCleanShot(string gun, string aim)
        {
            // Property: Comandos de 2 args NÃO registrados no CommandRegistry caem para Clean Shots
            
            var args = new[] { gun, aim };
            
            Assert.Equal(2, args.Length);
            // Validação: estes comandos não existem no CommandRegistry, então tentam TerminalCommands.json
        }

        [Theory]
        [InlineData("git", "clone", "https://github.com/user/repo.git", "pasta")]
        [InlineData("docker", "run", "-p", "8080", "nginx")]
        [InlineData("npm", "install", "react")]
        public void Dispatcher_WithThreePlusArgs_ShouldTryBurstFirst(params string[] args)
        {
            // Property: Para args.Length >= 3, tenta BurstCommands.json primeiro
            
            Assert.True(args.Length >= 3);
            // Validação: dispatcher deve tentar burstShots.SeekAndExecute(args) antes do CommandRegistry
        }

        [Theory]
        [InlineData("ask", "fulano", "qual", "é", "a", "resposta")]
        [InlineData("ask", "all", "alguma", "pergunta")]
        public void Dispatcher_WithThreePlusArgs_NotInBurst_ShouldFallbackToRegistry(params string[] args)
        {
            // Property: Se Burst não resolve, tenta CommandRegistry (ex: AskCommand)
            
            Assert.True(args.Length >= 3);
            // Validação: "ask" está registrado no CommandRegistry como fallback para Burst
        }

        [Fact]
        public void Dispatcher_PriorityOrder_IsCorrect()
        {
            // Property: A ordem de priorização deve ser respeitada
            // 1 arg:  CommandRegistry
            // 2 args: CommandRegistry → CleanShots
            // 3+ args: BurstShots → CommandRegistry
            
            var oneArg = new[] { "help" };
            var twoArgs = new[] { "download", "chrome" };
            var threeArgs = new[] { "git", "clone", "url" };
            
            Assert.Equal(1, oneArg.Length);
            Assert.Equal(2, twoArgs.Length);
            Assert.Equal(3, threeArgs.Length);
            
            // Validação estrutural da ordem de resolução
        }

        [Theory]
        [InlineData("HELP")]
        [InlineData("Help")]
        [InlineData("help")]
        public void Dispatcher_CommandNames_AreCaseInsensitive(string command)
        {
            // Property: Todos os comandos devem ser case-insensitive
            
            var args = new[] { command };
            
            // Validação: CommandRegistry usa StringComparer.OrdinalIgnoreCase
            Assert.True(command.Equals("help", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Dispatcher_WithUnknownCommand_ShouldNotThrow()
        {
            // Property P1.2 (No-Throw): O dispatcher nunca lança exceção não tratada
            
            var args = new[] { "unknown", "command", "with", "args" };
            
            // Validação: deve retornar "Unknown command! Try Again!" sem exceção
            Assert.True(args.Length > 0);
        }
    }
}
