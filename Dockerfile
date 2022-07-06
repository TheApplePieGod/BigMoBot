#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update
RUN apt-get install -y npm
WORKDIR /src
COPY ["BigMoBot.csproj", "."]
RUN dotnet restore "./BigMoBot.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BigMoBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BigMoBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BigMoBot.dll"]