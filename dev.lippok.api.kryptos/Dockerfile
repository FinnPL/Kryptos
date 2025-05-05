FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dev.lippok.api.kryptos/dev.lippok.api.kryptos.csproj", "dev.lippok.api.kryptos/"]
RUN dotnet restore "dev.lippok.api.kryptos/dev.lippok.api.kryptos.csproj"
COPY . .
WORKDIR "/src/dev.lippok.api.kryptos"
RUN dotnet build "./dev.lippok.api.kryptos.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./dev.lippok.api.kryptos.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dev.lippok.api.kryptos.dll"]
