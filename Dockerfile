FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DotnetEcommerce.Domain/DotnetEcommerce.Domain.csproj", "src/DotnetEcommerce.Domain/"]
COPY ["src/DotnetEcommerce.Application/DotnetEcommerce.Application.csproj", "src/DotnetEcommerce.Application/"]
COPY ["src/DotnetEcommerce.Infrastructure/DotnetEcommerce.Infrastructure.csproj", "src/DotnetEcommerce.Infrastructure/"]
COPY ["src/DotnetEcommerce.WebApi/DotnetEcommerce.WebApi.csproj", "src/DotnetEcommerce.WebApi/"]
RUN dotnet restore "src/DotnetEcommerce.WebApi/DotnetEcommerce.WebApi.csproj"

COPY . .
WORKDIR "/src/src/DotnetEcommerce.WebApi"
RUN dotnet build "DotnetEcommerce.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotnetEcommerce.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetEcommerce.WebApi.dll"]
