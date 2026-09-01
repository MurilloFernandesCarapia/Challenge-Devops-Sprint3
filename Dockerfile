
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source


COPY ["src/PetCare360.API/PetCare360.API.csproj", "src/PetCare360.API/"]
COPY ["src/PetCare360.Application/PetCare360.Application.csproj", "src/PetCare360.Application/"]
COPY ["src/PetCare360.Domain/PetCare360.Domain.csproj", "src/PetCare360.Domain/"]
COPY ["src/PetCare360.Infrastructure/PetCare360.Infrastructure.csproj", "src/PetCare360.Infrastructure/"]


RUN dotnet restore "src/PetCare360.API/PetCare360.API.csproj"


COPY . .

RUN dotnet publish "src/PetCare360.API/PetCare360.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app


RUN groupadd --system appuser && \
    useradd --system --gid appuser --create-home --shell /sbin/nologin appuser

COPY --from=build /app/publish .


RUN mkdir -p /app/logs && chown -R appuser:appuser /app


ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "PetCare360.API.dll"]