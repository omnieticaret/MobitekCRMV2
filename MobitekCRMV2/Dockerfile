#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MobitekCRMV2/MobitekCRMV2.csproj", "MobitekCRMV2/"]
COPY ["MobitekCRMV2.Authentication/MobitekCRMV2.Authentication.csproj", "MobitekCRMV2.Authentication/"]
COPY ["MobitekCRMV2.Entity/MobitekCRMV2.Entity.csproj", "MobitekCRMV2.Entity/"]
COPY ["MobitekCRMV2.Model/MobitekCRMV2.Model.csproj", "MobitekCRMV2.Model/"]
COPY ["MobitekCRMV2.Business/MobitekCRMV2.Business.csproj", "MobitekCRMV2.Business/"]
COPY ["MobitekCRMV2.DataAccess/MobitekCRMV2.DataAccess.csproj", "MobitekCRMV2.DataAccess/"]
COPY ["MobitekCRMV2.Dto/MobitekCRMV2.Dto.csproj", "MobitekCRMV2.Dto/"]
RUN dotnet restore "./MobitekCRMV2/MobitekCRMV2.csproj"
COPY . .
WORKDIR "/src/MobitekCRMV2"
RUN dotnet build "./MobitekCRMV2.csproj" -c Release -o /app/build

FROM build AS publish
EXPOSE 8080
RUN dotnet publish "./MobitekCRMV2.csproj" -c Release  -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MobitekCRMV2.dll"]
