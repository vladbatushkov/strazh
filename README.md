# strazh
Your code - is your Knowledge Graph

__WIP__

Release:

1.0.0-alpha < 1.0.0-alpha.1 < 1.0.0-alpha.beta < 1.0.0-beta < 1.0.0-beta.2 < 1.0.0-beta.11 < 1.0.0-rc.1 < 1.0.0

### docker

**Dockerfile**
```
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS sdk
WORKDIR /src
COPY Strazh Strazh/
RUN dotnet build /src/Strazh/Strazh.csproj -c Release -o /app
WORKDIR /app
ENV path=Project.csproj
ENV cred=neo4j:neo4j:test
CMD ["sh", "-c", "dotnet Strazh.dll -p $path -c $cred"]
```

In case you want to create a local `strazh:dev` image:

`docker build . -t strazh:dev`

Example how to run created `strazh:dev` container against the `Strazh.csproj` project (strazh can explore strazh codebase O_o )

`docker run -it --rm --network=host -v $(pwd)/Strazh:/dest -e path=/dest/Strazh.csproj -e cred=neo4j:neo4j:test strazh:dev`

_-- `docker volume` point to `Strazh` folder with `Strazh.csproj` and all the code inside._
_-- `path` environment used to point to project path inside docker container._
_-- `cred` environment used to connect to Neo4j with `database:user:password` credentials._

**docker-compose.yml**

Another one option to build and run all together.

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
      - path=/dest/Strazh.csproj
      - cred=neo4j:neo4j:strazh
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
