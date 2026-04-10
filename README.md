# HotelApi

Sistema de gestao hoteleira em .NET com arquitetura por camadas e API ASP.NET Core.

## Repositorio

- URL: https://github.com/Diassonama/HotelApi.git

## Estrutura principal

- `Hotel.Api/` API ASP.NET Core
- `Hotel.Application/` regras de aplicacao, comandos e queries
- `Hotel.Domain/` entidades e contratos de dominio
- `Hotel.Infrastruture/` persistencia, repositorios e configuracoes
- `Hotel.Identity/` autenticacao e identidade

## Pre-requisitos

- .NET SDK 8+
- SQL Server

## Como executar

1. Restaurar pacotes:
   - `dotnet restore Hotel.sln`
2. Compilar:
   - `dotnet build Hotel.sln`
3. Executar API:
   - `cd Hotel.Api`
   - `dotnet run`

## Endpoints

- Swagger (quando habilitado): `https://localhost:<porta>/swagger`
- Base local comum: `http://localhost:5055`

## Observacoes

- O projeto usa multi-tenant e configuracao de banco por tenant.
- Existem arquivos `.http` na raiz para testes de endpoints.
