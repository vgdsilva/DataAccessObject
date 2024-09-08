#!/bin/bash

# Caminhos baseados na pasta atual onde o script está localizado
BASE_PATH="$(dirname "$0")"
PROJECT_PATH="$BASE_PATH/projeto_principal"   # Substitua por sua pasta do projeto principal
POSTGRES_PATH="$BASE_PATH/PostgreSQL"         # Substitua pela pasta do projeto PostgreSQL
SQLITE_PATH="$BASE_PATH/SQLite"               # Substitua pela pasta do projeto SQLite

# Pasta de saída para os arquivos .dll
OUTPUT_DIR="DataAccessObjetcPostgreSQL"
SQLITE_OUTPUT_DIR="DataAccessObjetcSQLite"

# Função para compilar o projeto e mover o .dll para o diretório de saída
build_project() {
    PROJECT=$1
    OUTPUT=$2
    dotnet build "$PROJECT" -c Release

    # Extrair o nome do arquivo .dll (usando o nome do projeto)
    DLL_NAME=$(basename "$PROJECT").dll

    # Caminho do arquivo .dll gerado
    DLL_PATH="$PROJECT/bin/Release/netX.Y/$DLL_NAME"  # Ajuste netX.Y para sua versão alvo

    # Criar a pasta de saída se não existir
    mkdir -p "$OUTPUT"

    # Copiar o arquivo .dll para o diretório de saída
    cp "$DLL_PATH" "$OUTPUT"
}

# Compilar o projeto principal e copiar o .dll
build_project "$PROJECT_PATH" "$OUTPUT_DIR"

# Compilar o projeto PostgreSQL e copiar o .dll para a mesma pasta do projeto principal
build_project "$POSTGRES_PATH" "$OUTPUT_DIR"

# Compilar o projeto SQLite e copiar o .dll para o diretório específico
build_project "$SQLITE_PATH" "$SQLITE_OUTPUT_DIR"

echo "Builds concluídos e arquivos copiados para as pastas respectivas."
