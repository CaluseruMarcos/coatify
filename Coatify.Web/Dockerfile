FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Coatify.Web/Coatify.Web.csproj", "Coatify.Web/"]
COPY ["Coatify.Shared/Coatify.Shared.csproj", "Coatify.Shared/"]
RUN dotnet restore "Coatify.Web/Coatify.Web.csproj"
COPY . .
WORKDIR "/src/Coatify.Web"
RUN dotnet build "./Coatify.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Coatify.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN printf 'ok' > /app/publish/wwwroot/health

FROM nginx:1.27-alpine AS final
COPY Coatify.Web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 8080