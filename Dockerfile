FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Ruby.Server/Ruby.Server.csproj", "Ruby.Server/"]
COPY ["Ruby.BBL/Ruby.BBL.csproj", "Ruby.BBL/"]
COPY ["Ruby.DAL/Ruby.DAL.csproj", "Ruby.DAL/"]
COPY ["Ruby.Shared/Ruby.Shared.csproj", "Ruby.Shared/"]

RUN dotnet restore "Ruby.Server/Ruby.Server.csproj"


COPY . .


WORKDIR "/src/Ruby.Server"
RUN dotnet build "Ruby.Server.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "Ruby.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Ruby.Server.dll"]