# Burst Shots — Guia de Comandos com 3+ Argumentos

## O que são Burst Shots?

Comandos configuráveis via JSON que aceitam **3 ou mais argumentos**. Diferente dos Clean Shots (2 args), Burst Shots permitem comandos complexos como `git clone`, `docker run`, `npm install`, etc.

---

## Estrutura do JSON

Arquivo: `BurstCommands.json`

```json
[
  {
    "verb": "git",
    "action": "clone",
    "args": ["http*", "*"],
    "msg": "🔄 Clonando repositório Git...",
    "stp": [
      {
        "cmd": "git",
        "arg": "clone ${1} ${2}"
      }
    ]
  }
]
```

### Campos

| Campo | Descrição | Obrigatório |
|-------|-----------|-------------|
| `verb` | Verbo principal (ex: git, docker, npm) | ✅ |
| `action` | Ação/subcomando (ex: clone, run, install) | ✅ |
| `args` | Array de patterns para validar argumentos | ✅ |
| `msg` | Mensagem exibida ao usuário | ✅ |
| `stp` | Array de steps a executar | ✅ |

### Steps

| Campo | Descrição | Obrigatório |
|-------|-----------|-------------|
| `cmd` | Comando a executar | ✅ |
| `arg` | Argumentos do comando | ✅ |
| `mod` | Modo: `"interactive"` ou omitir | ❌ |

---

## Wildcards nos Args

| Pattern | Bate | Exemplo |
|---------|------|---------|
| `"*"` | Qualquer string | Aceita qualquer valor |
| `"http*"` | Começa com "http" | `https://...`, `http://...` |
| `"*.git"` | Termina com ".git" | `repo.git` |
| `"-p"` | Literal exato | Só aceita `-p` |

---

## Placeholders

Use `${1}`, `${2}`, `${3}`, etc nos steps para injetar os argumentos do usuário:

```json
{
  "verb": "git",
  "action": "clone",
  "args": ["http*", "*"],
  "stp": [
    {
      "cmd": "git",
      "arg": "clone ${1} ${2}"
    }
  ]
}
```

**Comando:** `maia git clone https://github.com/user/repo.git pasta`

**Argumentos extraídos:**
- `${1}` = `https://github.com/user/repo.git`
- `${2}` = `pasta`

**Executa:** `git clone https://github.com/user/repo.git pasta`

---

## Exemplos Práticos

### Git Clone

```json
{
  "verb": "git",
  "action": "clone",
  "args": ["http*", "*"],
  "msg": "🔄 Clonando repositório...",
  "stp": [
    {
      "cmd": "git",
      "arg": "clone ${1} ${2}"
    }
  ]
}
```

**Uso:** `maia git clone https://github.com/user/repo.git my-folder`

---

### Docker Run com Porta

```json
{
  "verb": "docker",
  "action": "run",
  "args": ["-p", "*", "*"],
  "msg": "🐳 Rodando container...",
  "stp": [
    {
      "cmd": "docker",
      "arg": "run ${1} ${2}:80 ${3}"
    }
  ]
}
```

**Uso:** `maia docker run -p 8080 nginx`

**Executa:** `docker run -p 8080:80 nginx`

---

### NPM Install

```json
{
  "verb": "npm",
  "action": "install",
  "args": ["*"],
  "msg": "📦 Instalando pacote npm...",
  "stp": [
    {
      "cmd": "npm",
      "arg": "install ${1}"
      }
  ]
}
```

**Uso:** `maia npm install react`

---

### SSH Connect (Modo Interativo)

```json
{
  "verb": "ssh",
  "action": "connect",
  "args": ["*", "*"],
  "msg": "🔐 Conectando via SSH...",
  "stp": [
    {
      "cmd": "ssh",
      "arg": "${1}@${2}",
      "mod": "interactive"
    }
  ]
}
```

**Uso:** `maia ssh connect user server.com`

**Executa:** `ssh user@server.com` (modo interativo)

---

## Diferença entre Modos

### Modo Padrão (omitir `mod`)
- Redireciona stdout/stderr
- Exibe saída em tempo real no console
- Não permite input do usuário

### Modo Interactive (`"mod": "interactive"`)
- Não redireciona streams
- Permite input do usuário (senhas, prompts)
- Útil para SSH, editores, prompts interativos

---

## Prioridade de Resolução (3+ args)

1. **BurstCommands.json** — tenta primeiro
2. **CommandRegistry** — fallback (ex: `maia ask fulano pergunta`)
3. Unknown command

---

## Como adicionar novos comandos

1. Edite `BurstCommands.json`
2. Adicione um objeto com `verb`, `action`, `args`, `msg`, `stp`
3. Use wildcards nos `args` para flexibilidade
4. Use placeholders `${N}` nos steps

**Não precisa recompilar o Maia!** 🎉

---

## Resumo das 3 Camadas

| Camada | Args | Arquivo | Exemplo |
|--------|------|---------|---------|
| Quick Shot | 1 | `CommandRegistry.cs` | `maia help` |
| Clean Shot | 2 | `TerminalCommands.json` | `maia download chrome` |
| Burst Shot | 3+ | `BurstCommands.json` | `maia git clone url pasta` |
