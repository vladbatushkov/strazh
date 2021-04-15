FROM mcr.microsoft.com/dotnet/sdk:3.1 AS sdk
WORKDIR /src
COPY Strazh/Strazh.csproj Strazh/Strazh.csproj
RUN dotnet restore /src/Strazh/Strazh.csproj

FROM sdk as build
WORKDIR /src
COPY Strazh Strazh/
RUN dotnet build /src/Strazh/Strazh.csproj -c Release -o /app
WORKDIR /app
ENV c=neo4j:neo4j:neo4j
ENV m=
ENV p=
ENV s=
CMD ["sh", "-c", "dotnet Strazh.dll -c $c -m $m -s $s -p $p"]