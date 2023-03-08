#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/WTA.Web/WTA.Web.csproj", "src/WTA.Web/"]
COPY ["src/Services/WTA.Application.BaseData/WTA.Application.BaseData.csproj", "src/Services/WTA.Application.BaseData/"]
COPY ["src/WTA.Application/WTA.Application.csproj", "src/WTA.Application/"]
COPY ["src/Services/WTA.Application.Identity/WTA.Application.Identity.csproj", "src/Services/WTA.Application.Identity/"]
COPY ["src/Services/WTA.Application.Wms/WTA.Application.Wms.csproj", "src/Services/WTA.Application.Wms/"]
COPY ["src/WTA.Infrastructure/WTA.Infrastructure.csproj", "src/WTA.Infrastructure/"]
RUN dotnet restore "src/WTA.Web/WTA.Web.csproj"
COPY . .
WORKDIR "/src/src/WTA.Web"
RUN dotnet build "WTA.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WTA.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WTA.Web.dll"]
