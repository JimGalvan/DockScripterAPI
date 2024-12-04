# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Define build argument
ARG ENVIRONMENT=Development

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY DockScripter/*.csproj ./DockScripter/
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Define build argument and set environment variable
ARG ENVIRONMENT=Development
ENV ASPNETCORE_ENVIRONMENT=$ENVIRONMENT

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose port and run the application
EXPOSE 80
ENTRYPOINT ["dotnet", "DockScripter.dll"]
