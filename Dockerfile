FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY HelpDesk.sln ./
COPY HelpDesk/HelpDesk.csproj HelpDesk/
COPY HelpDesk.Application/HelpDesk.Application.csproj HelpDesk.Application/
COPY HelpDesk.Domain/HelpDesk.Domain.csproj HelpDesk.Domain/
COPY HelpDesk.Infrastructure/HelpDesk.Infrastructure.csproj HelpDesk.Infrastructure/
RUN dotnet restore HelpDesk/HelpDesk.csproj

COPY . .
RUN dotnet publish HelpDesk/HelpDesk.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HelpDesk.dll"]