FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HM.Presentation.WebUI/HM.Presentation.WebUI.csproj", "HM.Presentation.WebUI/"]
COPY ["HM.Application/HM.Application.csproj", "HM.Application/"]
COPY ["HM.Domain/HM.Domain.csproj", "HM.Domain/"]
COPY ["HM.Infrastructure/HM.Infrastructure.csproj", "HM.Infrastructure/"]
RUN dotnet restore "HM.Presentation.WebUI/HM.Presentation.WebUI.csproj"
COPY . .
WORKDIR "/src/HM.Presentation.WebUI"
RUN dotnet build "./HM.Presentation.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HM.Presentation.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HM.Presentation.WebUI.dll"]
