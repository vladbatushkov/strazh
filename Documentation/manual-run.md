
# Manual run

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
dotnet build ./Strazh/Strazh.csproj -c Release -o ./app
```

##### Run Strazh

```
dotnet ./app/Strazh.dll -c neo4j:neo4j:strazh -p ./Strazh/Strazh.csproj
```

We point Strazh to build a Codebase Knowledge Graph of Strazh codebase :-)