# --- Build stage --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . . 

# Restore за всички проекти
RUN dotnet restore KazanlakRun.sln


# Публикувай само стартиращия проект
WORKDIR /src/KazanlakRun.Web
RUN dotnet publish -c Release -o /app/publish

# --- Runtime stage ------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

# Стартирай именно Web DLL-а
ENTRYPOINT ["dotnet", "KazanlakRun.Web.dll"]

