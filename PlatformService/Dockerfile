FROM mirror-docker.runflare.com/dotnet/sdk:10.0 AS build-environment

WORKDIR /app

COPY *.csproj ./
COPY ./nuget.config ./
RUN dotnet restore --force-evaluate
COPY . ./

RUN dotnet publish -c Release -o out

FROM mirror-docker.runflare.com/dotnet/aspnet:10.0.0
WORKDIR /app

COPY --from=build-environment /app/out .

ENTRYPOINT ["dotnet", "PlatformService.dll"]