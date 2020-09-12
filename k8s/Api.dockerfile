FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS builder
WORKDIR /app

# copying all csproj just because the sln needs it, but will just publish one of them
COPY ./src/Api/*.csproj ./src/Api/
COPY ./src/Contracts/*.csproj ./src/Contracts/
COPY ./HangfireSample.Api.sln ./
RUN dotnet restore

COPY ./src/Api/* ./src/Api/
COPY ./src/Contracts/* ./src/Contracts/
WORKDIR /app/src/Api/
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
COPY --from=builder /app/src/Api/out .
ENTRYPOINT ["dotnet", "Api.dll"]

# Sample build command (run from the repository root)
# docker build -t hangfire-sample/api:1.0 -f k8s/Api.dockerfile .