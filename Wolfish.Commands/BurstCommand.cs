using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Wolfish.Commands
{
    public class BurstCommand
    {
        private readonly string _path;

        public BurstCommand(string path)
        {
            _path = path;
        }

        /// <summary>
        /// Busca e executa um Burst Shot.
        /// Args esperado: ["verb", "action", "arg1", "arg2", ...]
        /// Exemplo: ["git", "clone", "https://...", "pasta"]
        /// </summary>
        public async Task<bool> SeekAndExecute(string[] userArgs)
        {
            if (userArgs.Length < 3)
                return false;

            if (!File.Exists(_path))
                return false;

            using var fileStream = File.OpenRead(_path);
            var commands = await JsonSerializer.DeserializeAsync<List<BurstCommandDto>>(fileStream);
            
            if (commands == null || commands.Count == 0)
                return false;

            var verb = userArgs[0];
            var action = userArgs[1];
            var userBurstArgs = userArgs.Skip(2).ToArray();

            // Buscar comando que bata verb + action
            var matchingCommands = commands.Where(c => 
                c.Verb?.Equals(verb, StringComparison.OrdinalIgnoreCase) == true &&
                c.Action?.Equals(action, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();

            if (matchingCommands.Count == 0)
                return false;

            // Encontrar comando onde os args batem
            BurstCommandDto? current = null;

            foreach (var cmd in matchingCommands)
            {
                if (cmd.Args == null || cmd.Args.Count == 0)
                {
                    // Sem args definidos = aceita qualquer coisa
                    current = cmd;
                    break;
                }

                if (ArgsMatch(cmd.Args, userBurstArgs))
                {
                    current = cmd;
                    break;
                }
            }

            if (current == null)
                return false;

            // Executar steps
            Console.Clear();
            Console.WriteLine(current.Msg);
            Console.WriteLine("_________________________________________________________________________________");

            foreach (var step in current.Stp ?? new List<BurstStep?>())
            {
                if (step == null)
                    continue;

                // Substituir placeholders ${1}, ${2}, etc
                var cmd = step.Cmd ?? "";
                var arg = step.Arg ?? "";

                for (int i = 0; i < userBurstArgs.Length; i++)
                {
                    var placeholder = $"${{{i + 1}}}";
                    cmd = cmd.Replace(placeholder, userBurstArgs[i]);
                    arg = arg.Replace(placeholder, userBurstArgs[i]);
                }

                switch (step.Mod?.ToLowerInvariant())
                {
                    case "interactive":
                        ExecuteInteractive(cmd, arg);
                        break;
                    default:
                        ExecuteRedirected(cmd, arg);
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se os args do JSON batem com os args do usuário.
        /// Suporta wildcards: "*", "http*", "*.git"
        /// </summary>
        private bool ArgsMatch(List<string> jsonArgs, string[] userArgs)
        {
            if (jsonArgs.Count != userArgs.Length)
                return false;

            for (int i = 0; i < jsonArgs.Count; i++)
            {
                var pattern = jsonArgs[i];
                var value = userArgs[i];

                if (pattern == "*")
                    continue;

                if (pattern.EndsWith("*"))
                {
                    var prefix = pattern.TrimEnd('*');
                    if (!value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        return false;
                    continue;
                }

                if (pattern.StartsWith("*"))
                {
                    var suffix = pattern.TrimStart('*');
                    if (!value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                        return false;
                    continue;
                }

                if (!pattern.Equals(value, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        private void ExecuteInteractive(string command, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();
            process.WaitForExit();
        }

        private void ExecuteRedirected(string command, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                StandardOutputEncoding = Encoding.UTF8,
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            using var process = new Process { StartInfo = startInfo };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.CursorVisible = false;
                    Console.CursorLeft = 0;
                    Console.Write(e.Data + "                                         ");
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.CursorVisible = false;
                    Console.CursorLeft = 0;
                    Console.Write(e.Data + "\n");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}

