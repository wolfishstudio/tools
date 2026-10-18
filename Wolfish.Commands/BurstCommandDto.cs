using System.Text.Json.Serialization;

namespace Wolfish.Commands
{
    /// <summary>
    /// DTO para Burst Shots — comandos com 3 ou mais argumentos definidos via JSON.
    /// Exemplo: "maia git clone https://... pasta" ou "maia docker run -p 8080 nginx"
    /// </summary>
    public class BurstCommandDto
    {
        /// <summary>
        /// Verbo principal do comando (ex: "git", "docker", "npm")
        /// </summary>
        [JsonPropertyName("verb")]
        public string? Verb { get; set; }

        /// <summary>
        /// Subcomando ou ação (ex: "clone", "run", "install")
        /// </summary>
        [JsonPropertyName("action")]
        public string? Action { get; set; }

        /// <summary>
        /// Definição dos argumentos esperados. Cada item pode ser:
        /// - "*" = aceita qualquer valor
        /// - "http*" = aceita strings que começam com "http"
        /// - "*.git" = aceita strings que terminam com ".git"
        /// - "-p" = literal, só aceita exatamente "-p"
        /// </summary>
        [JsonPropertyName("args")]
        public List<string>? Args { get; set; }

        /// <summary>
        /// Mensagem exibida ao usuário antes de executar
        /// </summary>
        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        /// <summary>
        /// Steps de execução. Suporta placeholders ${1}, ${2}, ${3} para injetar args do usuário.
        /// </summary>
        [JsonPropertyName("stp")]
        public List<BurstStep?>? Stp { get; set; }
    }

    public class BurstStep
    {
        /// <summary>
        /// Modo de execução: "interactive" ou omitir para padrão (redirect)
        /// </summary>
        [JsonPropertyName("mod")]
        public string? Mod { get; set; }

        /// <summary>
        /// Comando a executar. Suporta placeholders ${1}, ${2}, etc.
        /// </summary>
        [JsonPropertyName("cmd")]
        public string? Cmd { get; set; }

        /// <summary>
        /// Argumentos do comando. Suporta placeholders ${1}, ${2}, etc.
        /// </summary>
        [JsonPropertyName("arg")]
        public string? Arg { get; set; }
    }
}

