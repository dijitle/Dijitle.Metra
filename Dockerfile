FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
EXPOSE 80


FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

COPY ["src/Dijitle.Metra.API/Dijitle.Metra.API.csproj", "src/Dijitle.Metra.API/"]
COPY ["src/Dijitle.Metra.API.Models/Dijitle.Metra.API.Models.csproj", "src/Dijitle.Metra.API.Models/"]
COPY ["src/Dijitle.Metra.Data/Dijitle.Metra.Data.csproj", "src/Dijitle.Metra.Data/"]

RUN dotnet restore "src/Dijitle.Metra.API.Models/Dijitle.Metra.API.Models.csproj"
RUN dotnet restore "src/Dijitle.Metra.API/Dijitle.Metra.API.csproj"
RUN dotnet restore "src/Dijitle.Metra.Data/Dijitle.Metra.Data.csproj"

COPY . .

RUN dotnet build "Dijitle.Metra.sln" \
--configuration Release \
--no-restore

RUN dotnet publish "src/Dijitle.Metra.API/Dijitle.Metra.API.csproj" \
--configuration Release \
--no-build \
--no-restore \
--output /app

FROM base AS final
WORKDIR /app
COPY --from=build /app /app
ENTRYPOINT ["dotnet", "Dijitle.Metra.API.dll"]