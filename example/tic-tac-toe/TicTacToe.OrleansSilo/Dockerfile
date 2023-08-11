FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 30000

ENV ASPNETCORE_URLS=http://+:30000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TicTacToe.OrleansSilo/TicTacToe.OrleansSilo.csproj", "TicTacToe.OrleansSilo/"]
RUN dotnet restore "TicTacToe.OrleansSilo/TicTacToe.OrleansSilo.csproj"
COPY . .
WORKDIR "/src/TicTacToe.OrleansSilo"
RUN dotnet build "TicTacToe.OrleansSilo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicTacToe.OrleansSilo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 30000
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicTacToe.OrleansSilo.dll"]
