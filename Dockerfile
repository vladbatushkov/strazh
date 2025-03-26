FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS sdk
RUN mkdir -p /src/Stahz
WORKDIR /src
COPY Strazh/Strazh.csproj Strazh/Strazh.csproj
RUN dotnet restore /src/Strazh/Strazh.csproj

FROM sdk as build
RUN mkdir -p src
WORKDIR /src
COPY Strazh Strazh/
RUN dotnet build /src/Strazh/Strazh.csproj -c Release -o /app
WORKDIR /app
ENV c="neo4j:neo4j:neo4j"
ENV t="all"
ENV s="none"
ENV p=""
CMD ["sh", "-c", "dotnet Strazh.dll -c $c -t $t -s $s -p $p"]