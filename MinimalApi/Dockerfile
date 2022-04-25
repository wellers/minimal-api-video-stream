FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY MinimalApi.csproj ./MinimalApi/MinimalApi.csproj
RUN cd MinimalApi && dotnet restore

# Copy everything else and build app
COPY . ./MinimalApi/
WORKDIR /source/MinimalApi
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app ./
ENTRYPOINT ["dotnet", "MinimalApi.dll"]