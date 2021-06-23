# Strazh
Your codebase - is your Knowledge Graph.

### Manual run

##### Prerequisite

Run Neo4j on your local (for example, using docker)

```
docker run \
    --name strazh_neo4j \
    -p7474:7474 -p7687:7687 \
    -d \
    --env NEO4J_AUTH=neo4j/strazh \
    neo4j:latest
```

##### Build Strazh

Do fork & clone repository

```
dotnet build ./Strazh/Strazh.csproj -c Release -o /app
```

##### Run Strazh

```
dotnet ./app/Strazh.dll -c neo4j:neo4j:strazh -p ./Strazh/Strazh.csproj
```

We point Strazh to build a Codebase Knowledge Graph of Strazh codebase :-)

#### cli

```
Usage:
  strazh [options]
Options:
  -c, --credentials (REQUIRED)
required information in format `dbname:user:password` to connect to Neo4j Database
  -t, --tier
optional flag as `project` or `code` or 'all' (default `all`) selected tier to scan in a codebase
  -d, --delete
optional flag as `true` or `false` or no flag (default `true`) to delete data in graph before execution
  -s, --solution (REQUIRED)
optional absolute path to only one `.sln` file (can't be used together with -p / --projects)
  -p, --projects (REQUIRED)
optional list of absolute path to one or many `.csproj` files (can't be used together with -s / --solution)
  --version                                     
show version information
  -?, -h, --help                                
show help and usage information
```

### Run with docker-compose

```
version: '3'
services:

  strazh:
    build: .
    image: strazh:dev
    container_name: strazh
    network_mode: host
    volumes:
      - ./Strazh:/dest
    environment:
      - c=neo4j:neo4j:strazh
      - p=/dest/Strazh.csproj
    depends_on:
      - neo4j

  neo4j:
    image: neo4j:latest
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

You can also use a `latest` image instead
```
image: vladbatushkov/strazh:latest
```