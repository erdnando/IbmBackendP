#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Algar.Hours.Api/Algar.Hours.Api.csproj", "src/Algar.Hours.Api/"]
COPY ["src/Algar.Hours.Common/Algar.Hours.Common.csproj", "src/Algar.Hours.Common/"]
COPY ["src/Algar.Hours.Domain.Application/Algar.Hours.Application.csproj", "src/Algar.Hours.Domain.Application/"]
COPY ["src/Algar.Hours.Domain/Algar.Hours.Domain.csproj", "src/Algar.Hours.Domain/"]
COPY ["src/Algar.Hours.External/Algar.Hours.External.csproj", "src/Algar.Hours.External/"]
COPY ["src/Algar.Hours.Persistence/Algar.Hours.Persistence.csproj", "src/Algar.Hours.Persistence/"]
RUN dotnet restore "src/Algar.Hours.Api/Algar.Hours.Api.csproj"
COPY . .
WORKDIR "/src/src/Algar.Hours.Api"
RUN dotnet build "Algar.Hours.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Algar.Hours.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Algar.Hours.Api.dll"]