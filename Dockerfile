FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
EXPOSE 5147
# copy all the layers' csproj files into respective folders
COPY ["./Inventory/Inventory.Host.csproj", "src/Inventory/"]
COPY ["./Inventory.Application/Inventory.Application.csproj", "src/Inventory.Application/"]
COPY ["./Inventory.Consumer/Inventory.Consumer.csproj", "src/Inventory.Consumer/"]
COPY ["./Inventory.Domain/Inventory.Domain.csproj", "src/Inventory.Domain/"]
COPY ["./Inventory.Infractracture/Inventory.Infractracture.csproj", "src/Inventory.Infractracture/"]
COPY ["./Inventory.WebApi/Inventory.WebApi.csproj", "src/Inventory.WebApi/"]


# run restore over API project - this pulls restore over the dependent projects as well
RUN dotnet restore "src/Inventory/Inventory.Host.csproj"

COPY . .

# run build over the API project
WORKDIR "/src/Inventory/"
RUN dotnet build -c Release -o /app/build

# run publish over the API project
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS runtime
WORKDIR /app

COPY --from=publish /app/publish .
RUN ls -l
ENTRYPOINT [ "dotnet", "Inventory.Host.dll" ]