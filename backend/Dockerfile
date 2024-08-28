FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app

# Copy csproj and restore as distinct layers for UserAPI
COPY . /app/
RUN dotnet restore "UserAPI.csproj"

# Copy everything else and build for UserAPI
COPY ./ ./
WORKDIR "/app/"
RUN dotnet publish -c Release -o /app/publish/

# Create audit folder
RUN mkdir -p /app/audits

# Set the working directory to the published app
WORKDIR /app/publish/

# Install net-tools and procps
RUN apt-get update && apt-get install -y net-tools procps

# Set the entry point to run the UserAPI application
ENTRYPOINT ["dotnet", "UserAPI.dll"]

# Expose the correct port
EXPOSE 5000

# Set ASPNETCORE_URLS environment variable
ENV ASPNETCORE_URLS=http://+:5000