# Use the official .NET SDK as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project files to the container
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the project
RUN dotnet publish -c Release -o out

# Use the official .NET runtime as the runtime environment
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the compiled output from the build stage
COPY --from=build /app/out .

# Set environment variable for log path (can be overridden by user)
ENV LOG_PATH=/app/logs

# Set the entry point with arguments support
ENTRYPOINT ["dotnet", "LogParserV2.dll"]
