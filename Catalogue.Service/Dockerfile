FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Catalogue.Service/Catalogue.Service.sln", "./Catalogue.Service/"]
COPY ["Catalogue.Service/Catalogue.Service/Catalogue.Service.csproj", "Catalogue.Service/"]
COPY ["Catalogue.Service/Catalogue.Service.Data/Catalogue.Service.Data.csproj", "Catalogue.Service.Data/"]
COPY ["Catalogue.Service/Catalogue.Service.Models/Catalogue.Service.Models.csproj", "Catalogue.Service.Models/"]

RUN dotnet restore "Catalogue.Service/Catalogue.Service.sln"

COPY . .

WORKDIR "/src/Catalogue.Service"

RUN dotnet build "Catalogue.Service.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalogue.Service.sln" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Catalogue.Service.dll"]
