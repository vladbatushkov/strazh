# strazh
Your code - is your Knowledge Graph.

__WIP__

Release plan:

- 1.0.0-alpha
- 1.0.0-alpha.1..N <-- HERE NOW
- 1.0.0-beta
- 1.0.0-beta.1..N
- 1.0.0

### local

Build `strazh` from repository root:
```
dotnet build ./Strazh/Strazh.csproj -c Release -o /app
```
Run `strazh` to analyze `Strazh.csproj`:
```
dotnet ./app/Strazh.dll -c neo4j:neo4j:strazh -p ./Strazh/Strazh.csproj
```

### docker

**Dockerfile**
```
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS sdk
WORKDIR /src
COPY Strazh/Strazh.csproj Strazh/Strazh.csproj
RUN dotnet restore /src/Strazh/Strazh.csproj

FROM sdk as build
WORKDIR /src
COPY Strazh Strazh/
RUN dotnet build /src/Strazh/Strazh.csproj -c Release -o /app
WORKDIR /app
ENV cred=none
ENV path=None.csproj
CMD ["sh", "-c", "dotnet Strazh.dll -c $cred -p $path"]
```

In case you want to create a local `strazh:dev` image:

`docker build . -t strazh:dev`

Example how to run created `strazh:dev` container against the `Strazh.csproj` project (strazh can explore strazh codebase O_o )

`docker run -it --rm --network=host -v $(pwd)/Strazh:/dest -e cred=neo4j:neo4j:strazh -e path=/dest/Strazh.csproj strazh:dev`

_- `docker volume` point to `Strazh` folder with `Strazh.csproj` and all the code inside._
_- `cred` environment used to connect to Neo4j with `database:user:password` credentials._
_- `path` environment used to point to project path inside docker container._

**docker-compose.yml**

Another one option to build and run `strazh` is to use next `docker-compose` config:
```
version: '3'
services:

  strazh:
    build: .
    image: vladbatushkov/strazh:1.0.0-alpha.1
    container_name: strazh
    network_mode: host
    volumes:
      - ./Strazh:/dest
    environment:
      - cred=neo4j:neo4j:strazh
      - path=/dest/Strazh.csproj
    depends_on:
      - neo4j

  neo4j:
    image: neo4j:4.2
    container_name: strazh_neo4j
    restart: unless-stopped
    ports:
      - 7474:7474
      - 7687:7687
    environment:
      NEO4J_AUTH: neo4j/strazh
      NEO4J_dbms_memory_pagecache_size: 1G
      NEO4J_dbms.memory.heap.initial_size: 1G
      NEO4J_dbms_memory_heap_max__size: 1G
``` 
