FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY GameOfLife.Common/GameOfLife.Common.csproj GameOfLife.Common/
COPY GameOfLife.Engine/GameOfLife.Engine.csproj GameOfLife.Engine/
COPY GameOfLife.Api/GameOfLife.Api.csproj GameOfLife.Api/
RUN dotnet restore GameOfLife.Api/GameOfLife.Api.csproj

COPY GameOfLife.Common/ GameOfLife.Common/
COPY GameOfLife.Engine/ GameOfLife.Engine/
COPY GameOfLife.Api/ GameOfLife.Api/
RUN dotnet publish GameOfLife.Api/GameOfLife.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=5168
EXPOSE 5168
ENTRYPOINT ["dotnet", "GameOfLife.Api.dll"]
