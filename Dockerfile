FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["APIs/ProductCatalogService/ProductCatalogService.csproj", "APIs/ProductCatalogService/"]
RUN dotnet restore "APIs/ProductCatalogService/ProductCatalogService.csproj"

COPY . .
WORKDIR "/src/APIs/ProductCatalogService"
RUN dotnet build "ProductCatalogService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductCatalogService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
EXPOSE 5106
ENV ASPNETCORE_URLS=http://+:5106

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalogService.dll"]
