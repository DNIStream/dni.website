# Web API Dot Net Core Web Server Dockerfile

# ----------------
FROM microsoft/dotnet:2.1-sdk-alpine3.7 AS restore

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

COPY . ./

RUN for dir in test/*.Tests/; do (cd "$dir" && dotnet test --filter TestType!=Integration); done

WORKDIR /source/${PROJECT_DIR}

RUN dotnet build ${PROJECT_NAME}.csproj -c $ASPNET_CONFIGURATION -o /app

RUN dotnet publish ${PROJECT_NAME}.csproj -c $ASPNET_CONFIGURATION -o /app --no-restore

FROM microsoft/dotnet:2.1.2-aspnetcore-runtime-alpine3.7 as final
WORKDIR /app
EXPOSE 80
VOLUME /app/logs
VOLUME /app/files

COPY --from=build /app .

ENTRYPOINT ["dotnet", "DNI.API.dll"]
