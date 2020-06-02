#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/StackExchange.Redis.Branch.Samples/RealEstate/RealEstate.csproj", "src/StackExchange.Redis.Branch.Samples/RealEstate/"]
COPY ["src/StackExchange.Redis.Branch/StackExchange.Redis.Branch.csproj", "src/StackExchange.Redis.Branch/"]
RUN dotnet restore "src/StackExchange.Redis.Branch.Samples/RealEstate/RealEstate.csproj"
COPY . .
WORKDIR "/src/src/StackExchange.Redis.Branch.Samples/RealEstate"
RUN dotnet build "RealEstate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealEstate.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealEstate.dll"]
