# strazh
Your code - is your Knowledge Graph

__WIP__

Release:

1.0.0-alpha < 1.0.0-alpha.1 < 1.0.0-alpha.beta < 1.0.0-beta < 1.0.0-beta.2 < 1.0.0-beta.11 < 1.0.0-rc.1 < 1.0.0

### docker

**Dockerfile**
```
FROM mcr.microsoft.com/dotnet/sdk:3.1
WORKDIR /src
COPY Strazh Strazh
RUN dotnet build /src/Strazh/Strazh.csproj -c Release -o /app
WORKDIR /app
ENV path=default
CMD ["sh", "-c", "dotnet Strazh.dll $path"]
```

Create `strazh:dev` image:

`docker build . -t strazh:dev`

Run `strazh:dev` container against the `Strazh` project (strazh can explore self codebase, why not):

`docker run -it --rm --network=host -v $(pwd)/Strazh:/dest -e path=/dest/Strazh.csproj strazh:dev`

_-- `docker volume` point to `Strazh` folder with `Strazh.csproj` in it._
_-- `path` environment used to point to project path inside docker container._

**docker-compose.yml**
```
version: '3'
services:

  strazh:
    build:
      context: .
      dockerfile: ./Dockerfile
    image: strazh:dev
    container_name: strazh_dev
    network_mode: host
    volumes:
      - ./Strazh:/dest
    environment:
      - path=/dest/Strazh.csproj
    depends_on:
      - neo4j

  neo4j:
    image: neo4j:4.2
    container_name: strazh_neo4j_dev
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
