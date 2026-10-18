# 🧪 Suite de Testes — Wolfish.Maia

## 📊 Resumo Executivo

| Categoria | Arquivo | Testes | Foco |
|-----------|---------|--------|------|
| **Unitários** | `BurstCommandTests.cs` | 17 | Burst Shots, wildcards, matching |
| **Unitários** | `ProviderCommandTests.cs` | 4 | Comando provider, subcomandos |
| **Unitários** | `CommandRegistryTests.cs` | 7 | Registro, busca, case-insensitivity |
| **Integração** | `DispatcherIntegrationTests.cs` | 10 | Dispatcher 3 camadas, priorização |
| **Propriedades** | `PropertyTests.cs` | 9 | Invariantes universais |
| **TOTAL** | — | **47** | — |

---

## 🎯 Cobertura de Funcionalidades

### ✅ Burst Shots (BurstCommand)

| Funcionalidade | Teste | Status |
|----------------|-------|--------|
| Args mínimos (3+) | `SeekAndExecute_WithLessThan3Args_ReturnsFalse` | ✅ |
| Arquivo inexistente | `SeekAndExecute_WithNonExistentFile_ReturnsFalse` | ✅ |
| Verb não encontrado | `SeekAndExecute_WithNonMatchingVerb_ReturnsFalse` | ✅ |
| Action não encontrada | `SeekAndExecute_WithNonMatchingAction_ReturnsFalse` | ✅ |
| Wildcard simples `*` | `SeekAndExecute_WithSimpleWildcard_ReturnsTrue` | ✅ |
| Múltiplos wildcards | `SeekAndExecute_WithMultipleWildcards_ReturnsTrue` | ✅ |
| Contagem de args errada | `SeekAndExecute_WithWrongArgCount_ReturnsFalse` | ✅ |
| Prefix wildcard `http*` | `SeekAndExecute_WithPrefixWildcard_MatchesCorrectly` | ✅ |
| Suffix wildcard `*.git` | `SeekAndExecute_WithSuffixWildcard_MatchesCorrectly` | ✅ |
| Literal exato `-p` | `SeekAndExecute_WithLiteralMatch_RequiresExactValue` | ✅ |
| Combinações válidas | `SeekAndExecute_WithValidCombinations_ReturnsTrue` (Theory) | ✅ |
| Combinações inválidas | `SeekAndExecute_WithInvalidCombinations_ReturnsFalse` (Theory) | ✅ |
| Serialização JSON | `BurstCommandDto_Serialization_WorksCorrectly` | ✅ |

### ✅ Provider Command

| Funcionalidade | Teste | Status |
|----------------|-------|--------|
| Nome correto | `ProviderCommand_Name_IsProvider` | ✅ |
| Sem subcomando → help | `ExecuteAsync_WithNoSubcommand_ShowsHelp` | ✅ |
| Subcomando inválido | `ExecuteAsync_WithInvalidSubcommand_ShowsError` | ✅ |
| Case-insensitive | `ExecuteAsync_SubcommandIsCaseInsensitive` | ✅ |

### ✅ Command Registry

| Funcionalidade | Teste | Status |
|----------------|-------|--------|
| Criar padrão | `CreateDefault_ContainsStandardCommands` | ✅ |
| Registrar comando | `Register_AddsCommandToRegistry` | ✅ |
| Comando desconhecido | `TryExecuteAsync_WithUnknownCommand_ReturnsFalse` | ✅ |
| Case-insensitive | `TryExecuteAsync_IsCaseInsensitive` | ✅ |
| Chaining | `Register_ReturnsRegistryForChaining` | ✅ |
| Comandos padrão | `DefaultRegistry_ContainsExpectedCommands` | ✅ |

### ✅ Dispatcher (Integração)

| Regra | Teste | Status |
|-------|-------|--------|
| 0 args → help | `Dispatcher_WithNoArgs_ShouldShowHelp` | ✅ |
| 1 arg → Registry | `Dispatcher_WithOneArg_ShouldTryQuickShot` | ✅ |
| 2 args → Registry primeiro | `Dispatcher_WithTwoArgs_QuickShotFirst_ShouldBeRegistryCommand` | ✅ |
| 2 args → Clean fallback | `Dispatcher_WithTwoArgs_NotInRegistry_ShouldTryCleanShot` | ✅ |
| 3+ args → Burst primeiro | `Dispatcher_WithThreePlusArgs_ShouldTryBurstFirst` | ✅ |
| 3+ args → Registry fallback | `Dispatcher_WithThreePlusArgs_NotInBurst_ShouldFallbackToRegistry` | ✅ |
| Ordem de prioridade | `Dispatcher_PriorityOrder_IsCorrect` | ✅ |
| Case-insensitive | `Dispatcher_CommandNames_AreCaseInsensitive` | ✅ |
| No-throw | `Dispatcher_WithUnknownCommand_ShouldNotThrow` | ✅ |

### ✅ Propriedades Invariantes

| Propriedade | Teste | Status |
|-------------|-------|--------|
| `*` aceita qualquer string | `Property_WildcardStar_AcceptsAnyString` | ✅ |
| `P*` bate prefixo | `Property_PrefixWildcard_MatchesCorrectly` | ✅ |
| `*S` bate sufixo | `Property_SuffixWildcard_MatchesCorrectly` | ✅ |
| Literal exato | `Property_LiteralPattern_RequiresExactMatch` | ✅ |
| Contagem de args | `Property_ArgsCount_MustMatch` | ✅ |
| Args vazios | `Property_EmptyArgs_NeverMatches` | ✅ |
| Burst mínimo 3 args | `Property_BurstMinimumArgs_IsThree` | ✅ |

---

## 🚀 Executar Testes

### Todos os testes
```bash
./run-tests.sh
```

ou

```bash
dotnet test Wolfish.Maia.Tests/
```

### Com detalhes verbosos
```bash
dotnet test Wolfish.Maia.Tests/ --logger "console;verbosity=detailed"
```

### Apenas uma classe
```bash
dotnet test --filter "FullyQualifiedName~BurstCommandTests"
```

### Apenas testes de propriedades
```bash
dotnet test --filter "FullyQualifiedName~PropertyTests"
```

### Com cobertura de código
```bash
dotnet test Wolfish.Maia.Tests/ --collect:"XPlat Code Coverage"
```

---

## 📈 Métricas de Qualidade

### Cobertura Esperada

| Componente | Cobertura Alvo | Status |
|------------|----------------|--------|
| BurstCommand | > 90% | 🎯 |
| CommandRegistry | > 85% | 🎯 |
| ProviderCommand | > 70% | 🎯 |
| Dispatcher Logic | > 80% | 🎯 |

### Tempo de Execução

- Suite completa: < 5 segundos
- Testes unitários: < 2 segundos
- Testes de integração: < 3 segundos

---

## 🐛 Casos de Borda Testados

1. ✅ Args vazios
2. ✅ Arquivo JSON inexistente
3. ✅ JSON malformado (via exceção esperada)
4. ✅ Comando não encontrado
5. ✅ Contagem de args errada
6. ✅ Wildcards extremos (`*`, `***`)
7. ✅ Case-insensitivity em todos os níveis
8. ✅ Literais com caracteres especiais
9. ✅ Prefixos/sufixos vazios após remover `*`
10. ✅ URLs com vários protocolos (http, https, ftp)

---

## 🔧 Testes Faltantes (Roadmap)

- [ ] Testes end-to-end com execução real de comandos
- [ ] Testes de performance (latência de resolução)
- [ ] Testes de concorrência (múltiplos comandos simultâneos)
- [ ] Testes de serialização com JSON inválido
- [ ] Testes de placeholders com valores edge-case (vazio, null, Unicode)
- [ ] Testes de modo interativo vs redirected
- [ ] Mocks de ProcessStartInfo para validar comandos sem executá-los

---

## 📝 Convenções de Nomenclatura

### Métodos de Teste

```
[Componente]_[Cenário]_[ResultadoEsperado]

Exemplos:
- SeekAndExecute_WithLessThan3Args_ReturnsFalse
- Dispatcher_WithNoArgs_ShouldShowHelp
- Property_WildcardStar_AcceptsAnyString
```

### Theory Data

```
[MemberData(nameof([Componente][Propósito]TestData))]

Exemplo:
- ArgsCountTestData
- ValidCombinationsTestData
```

---

## 🤝 Contribuindo

Ao adicionar novos testes:

1. **Escolha a categoria correta** (unitário, integração, propriedade)
2. **Siga a convenção de nomenclatura**
3. **Documente o que está sendo validado** (comentário `// Property:` ou `// Validates:`)
4. **Use Theory para testes parametrizados** quando possível
5. **Mantenha testes independentes** (sem dependência de ordem de execução)
