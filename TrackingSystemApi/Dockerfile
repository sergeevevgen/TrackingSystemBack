#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TrackingSystemApi/TrackingSystem.Api.csproj", "TrackingSystemApi/"]
RUN dotnet restore "TrackingSystemApi/TrackingSystem.Api.csproj"
COPY . .
WORKDIR "/src/TrackingSystemApi"
RUN dotnet build "TrackingSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrackingSystem.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrackingSystem.Api.dll"]