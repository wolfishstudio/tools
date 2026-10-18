using Wolfish.Commands;
using Wolfish.Maia.Commands;

namespace Wolfish.Maia
{
    public class Program
    {        
        private static async Task Main(string[] args)
        {
            var found = false;
            var baseDirectory = AppContext.BaseDirectory;
            
            // Inicializar os 3 tipos de comando
            var commandRegistry = CommandRegistry.CreateDefault();
            var cleanShots = new WolfishCommand(Path.Combine(baseDirectory, "TerminalCommands.json"));
            var burstShots = new BurstCommand(Path.Combine(baseDirectory, "BurstCommands.json"));

            // Sem argumentos: exibe ajuda
            if (args.Length == 0) 
            { 
                await commandRegistry.TryExecuteAsync("help", args); 
                return; 
            }

            // 1. Quick Shot (1 argumento) — CommandRegistry
            // Exemplos: maia help, maia welcome, maia list
            if (args.Length == 1)
            {
                found = await commandRegistry.TryExecuteAsync(args[0], args);
            }

            // 2. Clean Shot (2 argumentos) — TerminalCommands.json
            // Exemplos: maia download chrome, maia install node
            if (!found && args.Length == 2) 
            { 
                found = await cleanShots.SeekAndExecute(args[0], args[1]); 
            }

            // 3. Burst Shot (3+ argumentos) — BurstCommands.json primeiro, depois CommandRegistry
            // Exemplos: maia git clone https://... pasta, maia docker run -p 8080 nginx
            if (!found && args.Length >= 3) 
            { 
                // Tenta Burst JSON primeiro
                found = await burstShots.SeekAndExecute(args);
                
                // Se não achou no JSON, tenta CommandRegistry (ex: maia ask fulano pergunta)
                if (!found)
                {
                    found = await commandRegistry.TryExecuteAsync(args[0], args);
                }
            }

            // 4. Nada funcionou
            if (!found) 
            { 
                Console.WriteLine($"Unknown command! Try Again!"); 
            }
        }
    }
}

