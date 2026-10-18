#!/bin/bash

echo "================================"
echo "  Wolfish.Maia Test Runner"
echo "================================"
echo ""

cd "$(dirname "$0")"

echo "📦 Restaurando dependências..."
dotnet restore Wolfish.Maia.Tests/Wolfish.Maia.Tests.csproj

echo ""
echo "🔨 Compilando testes..."
dotnet build Wolfish.Maia.Tests/Wolfish.Maia.Tests.csproj --no-restore

echo ""
echo "🧪 Executando testes..."
dotnet test Wolfish.Maia.Tests/Wolfish.Maia.Tests.csproj --no-build --logger "console;verbosity=normal"

echo ""
echo "✅ Testes concluídos!"
