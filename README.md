# strazh
Your code - is your Knowledge Graph

### docker

Dockerfile:
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

`docker run -it --rm -v $(pwd)/Strazh:/dest -e path=/dest/Strazh.csproj strazh:dev`

_-- `docker volume` point to `Strazh` folder with `Strazh.csproj` in it._
_-- `path` environment used to point to project path inside docker container._
