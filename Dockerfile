FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY ./iot-api/ ./src
WORKDIR ./src
RUN dotnet restore
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM runtime AS final

ENV TZ America/New_York
ENV MONGO_PORT 27017
ENV TIMEOUT 1000
ENV RETRY_DELAY 500
ENV DEVICE_RETRIES 4

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "iot-api.dll"]
