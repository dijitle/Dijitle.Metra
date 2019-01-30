FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/Dijitle.Metra.API/Dijitle.Metra.API.csproj", "src/Dijitle.Metra.API/"]
RUN dotnet restore "src/Dijitle.Metra.API/Dijitle.Metra.API.csproj"
COPY . .
WORKDIR "/src/src/Dijitle.Metra.API"
RUN dotnet build "Dijitle.Metra.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Dijitle.Metra.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Dijitle.Metra.API.dll"]