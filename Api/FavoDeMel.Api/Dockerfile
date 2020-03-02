#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
COPY ["src/FavoDeMel.Api/FavoDeMel.Api.csproj", "./src/FavoDeMel.Api/"]
COPY ["src/FavoDeMel.IoC/FavoDeMel.IoC.csproj", "./src/FavoDeMel.IoC/"]
COPY ["src/FavoDeMel.Service/FavoDeMel.Service.csproj", "./src/FavoDeMel.Service/"]
COPY ["src/FavoDeMel.Repository/FavoDeMel.Repository.csproj", "./src/FavoDeMel.Repository/"]
COPY ["src/FavoDeMel.Domain/FavoDeMel.Domain.csproj", "./src/FavoDeMel.Domain/"]
COPY ["test/FavoDeMel.Tests", "./test/FavoDeMel.Tests/"]
RUN dotnet restore "FavoDeMel.Api/FavoDeMel.Api.csproj"
COPY . .
WORKDIR "/src/FavoDeMel.Api"
RUN dotnet build "FavoDeMel.Api.csproj" -c Release -o /app/build

FROM build AS testrunner
WORKDIR /app/test/FavoDeMel.Tests
CMD ["dotnet", "test", "--logger:trx"]

FROM build AS test
WORKDIR /app/test/FavoDeMel.Tests
RUN dotnet test --logger:trx

FROM build AS publish
RUN dotnet publish "FavoDeMel.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FavoDeMel.Api.dll"] 