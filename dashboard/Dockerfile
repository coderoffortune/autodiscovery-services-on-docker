FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env

WORKDIR /app

COPY ./dashboard/*.csproj ./

RUN dotnet restore

COPY ./dashboard ./

RUN dotnet publish -c Release -o output

# Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

WORKDIR /app

COPY --from=build-env /app/output .

CMD ["dotnet", "dashboard.dll"]
