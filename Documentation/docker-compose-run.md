
# Run with docker-compose

Example of docker-compose file with image `build`

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