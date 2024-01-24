#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Node.js
RUN apt-get update && \
    apt-get install -y nodejs npm && \
    node --version && npm --version

COPY ["boiler.client/nuget.config", "boiler.client/"]
COPY ["Boiler.Server/Boiler.Server.csproj", "Boiler.Server/"]
COPY ["boiler.client/boiler.client.esproj", "boiler.client/"]
COPY ["Boiler.Shared/Boiler.Shared.csproj", "Boiler.Shared/"]
COPY ["Boiler.Server/libs/libDlibDotNetNativeDnnAgeClassification.so", "/root/.nuget/packages/facerecognitiondotnet/1.3.0.7/runtimes/linux-x64/native/"]
COPY ["Boiler.Server/libs/libDlibDotNetNativeDnnEmotionClassification.so", "/root/.nuget/packages/facerecognitiondotnet/1.3.0.7/runtimes/linux-x64/native/"]
COPY ["Boiler.Server/libs/libDlibDotNetNativeDnnGenderClassification.so", "/root/.nuget/packages/facerecognitiondotnet/1.3.0.7/runtimes/linux-x64/native/"]
RUN dotnet restore "./Boiler.Server/./Boiler.Server.csproj"
COPY . .
WORKDIR "/src/Boiler.Server"
RUN dotnet build "./Boiler.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
COPY ["Boiler.Server/libs/FaceRecognitionDotNet.props", "/root/.nuget/packages/facerecognitiondotnet/1.3.0.7/build/"]
RUN dotnet publish "./Boiler.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
USER root
WORKDIR /app

# Create the /app/uploaded directory
RUN mkdir -p /app/uploaded
# Change ownership to 'app' user
RUN chown app:app /app/uploaded

# Switch back to 'app' user
USER app

# Copy from publish stage
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Boiler.Server.dll"]
