#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["cartao.servico/cartao.servico.csproj", "cartao.servico/"]
COPY ["cartao.core.domain/Cartao.Domain.csproj", "cartao.core.domain/"]
RUN dotnet restore "cartao.servico/cartao.servico.csproj"
COPY . .
WORKDIR "/src/cartao.servico"
RUN dotnet build "cartao.servico.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "cartao.servico.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cartao.servico.dll"]