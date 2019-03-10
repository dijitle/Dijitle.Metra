FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
EXPOSE 80


FROM microsoft/dotnet:2.2-sdk AS build

COPY ["src/Dijitle.Metra.API/Dijitle.Metra.API.csproj", "src/Dijitle.Metra.API/"]
RUN dotnet restore "src/Dijitle.Metra.API/Dijitle.Metra.API.csproj"

COPY ["src/Dijitle.Metra.API.Models/Dijitle.Metra.API.Models.csproj", "src/Dijitle.Metra.API.Models/"]
RUN dotnet restore "src/Dijitle.Metra.API.Models/Dijitle.Metra.API.Models.csproj"

COPY ["src/Dijitle.Metra.Data/Dijitle.Metra.Data.csproj", "src/Dijitle.Metra.Data/"]
RUN dotnet restore "src/Dijitle.Metra.Data/Dijitle.Metra.Data.csproj"

COPY . .

RUN dotnet build "Dijitle.Metra.sln" \
--configuration Release 

RUN dotnet publish "src/Dijitle.Metra.API/Dijitle.Metra.API.csproj" \
--configuration Release \
--no-build \
--no-restore \
--output /app

FROM base AS final
WORKDIR /app
COPY --from=build /app /app
ENTRYPOINT ["dotnet", "Dijitle.Metra.API.dll"]