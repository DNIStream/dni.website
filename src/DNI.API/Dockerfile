# Web API Dot Net Core Web Server Dockerfile

# ----------------
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS restore

WORKDIR /source

COPY ./*.sln ./nuget.config ./

# Copy source project files
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# Copy test project files
COPY test/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p test/${file%.*}/ && mv $file test/${file%.*}/; done

RUN dotnet restore

FROM restore AS build
ARG ASPNET_CONFIGURATION=
ARG PROJECT_DIR=
ARG PROJECT_NAME=

RUN echo $ASPNET_CONFIGURATION

COPY . ./

RUN for dir in test/*.Tests/; do (cd "$dir" && dotnet test --filter TestType!=Integration); done

WORKDIR /source/${PROJECT_DIR}

RUN dotnet publish ${PROJECT_NAME}.csproj --no-restore -c $ASPNET_CONFIGURATION -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine as final
WORKDIR /app
EXPOSE 80
VOLUME /app/logs

COPY --from=build /app .

ENTRYPOINT ["dotnet", "DNI.API.dll"]
