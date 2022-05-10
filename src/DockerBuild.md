# Build Docker Images

## Build steps

* Publish the release binary

```shell
docker build -t ast-cli .
```

## Docker image usage:

The container looks for input files in /data. So for example, if you have a `project.assets.json` defined on your system in `c:\src\project\obj`, you can use the container to generate a Mermaid(mmd) file as follows:

```shell
docker run -it -v c:\src\project\obj:/data saars/ast-cli
```
This will map `c:\src\project\obj` into `/data` in the container. And you will find the output file at:

```
c:\src\project\obj\output.mmd
```

