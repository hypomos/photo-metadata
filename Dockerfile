FROM mcr.microsoft.com/dotnet/core/runtime:3.0-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /src
COPY ["photo-metadata.csproj", ""]
RUN dotnet restore "photo-metadata.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "photo-metadata.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "photo-metadata.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "photo-metadata.dll"]