#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["api.aldea/api.aldea.csproj", "api.aldea/"]
RUN dotnet restore "api.aldea/api.aldea.csproj"
COPY . .
WORKDIR "/src/api.aldea"
RUN dotnet build "api.aldea.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api.aldea.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.aldea.dll"]