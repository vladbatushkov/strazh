FROM mcr.microsoft.com/dotnet/sdk:3.1 AS sdk
WORKDIR /src
COPY Strazh Strazh/
RUN dotnet build /src/Strazh/Strazh.csproj -c Release -o /app
WORKDIR /app
ENV path=default
CMD ["sh", "-c", "dotnet Strazh.dll $path"]