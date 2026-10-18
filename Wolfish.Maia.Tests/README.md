# Wolfish.Maia.Tests

Suite de testes unitários e de integração para o Wolfish.Maia.

## Estrutura

```
Wolfish.Maia.Tests/
├── BurstCommandTests.cs              # Testes unitários do BurstCommand
├── ProviderCommandTests.cs           # Testes do ProviderCommand
├── CommandRegistryTests.cs           # Testes do CommandRegistry
├── DispatcherIntegrationTests.cs     # Testes de integração do dispatcher
├── PropertyTests.cs                  # Testes baseados em propriedades
└── TestData/
    └── test-burst-commands.json      # Dados de teste para Burst
```

## Executar Testes

```bash
# Todos os testes
dotnet test

# Com detalhes
dotnet test --logger "console;verbosity=detailed"

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Apenas uma classe
dotnet test --filter "FullyQualifiedName~BurstCommandTests"

# Apenas um método
dotnet test --filter "FullyQualifiedName~BurstCommandTests.SeekAndExecute_WithSimpleWildcard_ReturnsTrue"
```

## Categorias de Testes

### 1. Testes Unitários

**BurstCommandTests** — Valida a lógica de Burst Shots:
- ✅ Resolução de comandos por verb + action
- ✅ Matching de wildcards (`*`, `http*`, `*.git`)
- ✅ Validação de contagem de argumentos
- ✅ Matching de literais exatos
- ✅ Serialização/deserialização JSON

**ProviderCommandTests** — Valida o comando `maia provider`:
- ✅ Nome do comando
- ✅ Subcomandos (list, add, edit)
- ✅ Validação de entrada
- ✅ Case-insensitivity

**CommandRegistryTests** — Valida o registro de comandos:
- ✅ Registro de comandos
- ✅ Busca case-insensitive
- ✅ Chaining de registro
- ✅ Comandos padrão presentes

### 2. Testes de Integração

**DispatcherIntegrationTests** — Valida a lógica de priorização do dispatcher:
- ✅ 1 arg → CommandRegistry
- ✅ 2 args → CommandRegistry → CleanShots
- ✅ 3+ args → BurstShots → CommandRegistry
- ✅ Prioridade correta entre camadas
- ✅ Case-insensitivity global

### 3. Testes de Propriedades

**PropertyTests** — Valida invariantes universais:
- ✅ Wildcard `*` aceita qualquer string
- ✅ Prefix wildcard `http*` funciona corretamente
- ✅ Suffix wildcard `*.git` funciona corretamente
- ✅ Literais requerem match exato (case-insensitive)
- ✅ Contagem de args deve bater
- ✅ Burst sempre tem ≥ 3 args

## Propriedades Validadas

### P1: Wildcard Universal
**Propriedade:** Para qualquer string S, o pattern "*" sempre bate.

**Testes:** `Property_WildcardStar_AcceptsAnyString`

### P2: Prefix Matching
**Propriedade:** Para pattern "P*", bate se e somente se value.StartsWith(P).

**Testes:** `Property_PrefixWildcard_MatchesCorrectly`

### P3: Suffix Matching
**Propriedade:** Para pattern "*S", bate se e somente se value.EndsWith(S).

**Testes:** `Property_SuffixWildcard_MatchesCorrectly`

### P4: Args Count Invariant
**Propriedade:** Para jsonArgs de tamanho N e userArgs de tamanho M, bate se e somente se N == M.

**Testes:** `Property_ArgsCount_MustMatch`

### P5: Burst Minimum Args
**Propriedade:** Burst Shots sempre têm args.Length >= 3.

**Testes:** `Property_BurstMinimumArgs_IsThree`

### P6: No-Throw Dispatcher
**Propriedade:** O dispatcher nunca lança exceção não tratada independente dos argumentos.

**Testes:** `Dispatcher_WithUnknownCommand_ShouldNotThrow`

## Cobertura de Código

Execute com cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Para visualizar (instale reportgenerator):

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## Contribuindo

Ao adicionar novos recursos:
1. Escreva testes unitários para a lógica isolada
2. Escreva testes de integração para o fluxo completo
3. Identifique e teste propriedades invariantes
4. Mantenha cobertura > 80%
