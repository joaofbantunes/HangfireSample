FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS builder
WORKDIR /app

# copying all csproj just because the sln needs it, but will just publish one of them
COPY ./src/Worker/*.csproj ./src/Worker/
COPY ./src/Contracts/*.csproj ./src/Contracts/
COPY ./HangfireSample.Worker.sln ./
RUN dotnet restore

COPY ./src/Worker/* ./src/Worker/
COPY ./src/Contracts/* ./src/Contracts/
WORKDIR /app/src/Worker/
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
COPY --from=builder /app/src/Worker/out .
RUN apk add libcap && setcap 'CAP_NET_BIND_SERVICE=+ep' /usr/share/dotnet/dotnet && apk del libcap
ENTRYPOINT ["dotnet", "Worker.dll"]

# Sample build command (run from the repository root)
# docker build -t hangfire-sample/worker:1.0 -f k8s/Worker.dockerfile .
