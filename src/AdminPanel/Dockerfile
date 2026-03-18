FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["src/AdminPanel/AdminPanel.csproj", "AdminPanel/"]
COPY ["src/InterStyle.Shared/InterStyle.Shared.csproj", "InterStyle.Shared/"]
RUN dotnet restore "AdminPanel/AdminPanel.csproj" --nologo

COPY ["src/AdminPanel/", "AdminPanel/"]
COPY ["src/InterStyle.Shared/", "InterStyle.Shared/"]

WORKDIR /src/AdminPanel
RUN dotnet publish "AdminPanel.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --nologo

FROM nginx:1.28-alpine AS final
WORKDIR /usr/share/nginx/html

COPY ["src/AdminPanel/nginx.conf", "/etc/nginx/conf.d/default.conf"]
COPY --from=publish /app/publish/wwwroot ./

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]