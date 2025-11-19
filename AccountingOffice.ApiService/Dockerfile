# See https://aka.ms/customizecontainer to learn how to customize your debug container
ARG LAUNCHING_FROM_VS
ARG FINAL_BASE_IMAGE=${LAUNCHING_FROM_VS:+aotdebug}

# Stage usada apenas no debug rápido do Visual Studio
FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0-alpine AS base
RUN apk add --no-cache wget
USER $APP_UID
WORKDIR /app
EXPOSE 8080 8081

# Build
FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AccountingOffice.ApiService/AccountingOffice.ApiService.csproj", "AccountingOffice.ApiService/"]
RUN dotnet restore "./AccountingOffice.ApiService/AccountingOffice.ApiService.csproj"
COPY . .
WORKDIR "/src/AccountingOffice.ApiService"
RUN dotnet build "./AccountingOffice.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AccountingOffice.ApiService.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=true

# Stage de debug com gdb (Visual Studio)
FROM base AS aotdebug
USER root
RUN apk update && apk add gdb
USER app

# =========================================
# STAGE FINAL – agora sobe 100%
# =========================================
FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0-alpine AS final

# Instala ICU completo (obrigatório no .NET 10 nightly Alpine)
RUN apk add --no-cache \
    icu-libs \
    icu-data-full \
    wget

# (Opcional) Define cultura invariante se quiser remover ICU completamente
# ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
EXPOSE 8080 8081

COPY --from=publish /app/publish .

# Debug útil: mostra que o .NET e ICU estão instalados
RUN dotnet --info

ENTRYPOINT ["./AccountingOffice.ApiService"]