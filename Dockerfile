FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Origin.Identity.slnx ./

COPY src/Origin.Identity.API/Origin.Identity.API.csproj src/Origin.Identity.API/
COPY src/Origin.Identity.Application/Origin.Identity.Application.csproj src/Origin.Identity.Application/
COPY src/Origin.Identity.Contracts/Origin.Identity.Contracts.csproj src/Origin.Identity.Contracts/
COPY src/Origin.Identity.Domain/Origin.Identity.Domain.csproj src/Origin.Identity.Domain/
COPY src/Origin.Identity.Infrastructure/Origin.Identity.Infrastructure.csproj src/Origin.Identity.Infrastructure/

RUN dotnet restore src/Origin.Identity.API/Origin.Identity.API.csproj

FROM node:24-alpine AS ui-build
WORKDIR /ui

COPY src/Origin.Identity.UI/package*.json ./
RUN npm ci

COPY src/Origin.Identity.UI/ ./

RUN npm run build -- --configuration production --base-href /auth/ --deploy-url /auth/

FROM build AS publish

COPY . .

RUN dotnet publish src/Origin.Identity.API/Origin.Identity.API.csproj -c Release -o /app/publish

RUN rm -rf /app/publish/wwwroot/auth && mkdir -p /app/publish/wwwroot/auth

COPY --from=ui-build /ui/dist/Origin.Identity.UI/browser/ /app/publish/wwwroot/auth/

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Origin.Identity.API.dll"]