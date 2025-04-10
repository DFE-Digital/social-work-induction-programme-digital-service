# Structurizr Lite in Docker

This shows how Stucturizer Lite can be used to the view and create Structurizr diagrams

## Docker Compose

```
services:
  c4:
    image: structurizr/lite
    restart: "unless-stopped"
    volumes:
      - ./swip-workspace.dsl:/usr/local/structurizr/workspace.dsl
      - ./icons:/usr/local/structurizr/icons
      - ./structurizr.properties:/usr/local/structurizr/structurizr.properties
    ports:
      - "8080:8080"
```

We use the `structurizr/lite` from the Docker Hub library.

`restart: "unless-stopped"` will restart a crashed container (not that we've seen that happen)

The key volume attaches the local workspace file into the expected file in the container `workspace.dsl`

```
volumes:
  - ./swip-workspace.dsl:/usr/local/structurizr/workspace.dsl
```

Any additional images or a properties file must also be mapped into the running container

```
volumes:
  - ./icons:/usr/local/structurizr/icons
  - ./structurizr.properties:/usr/local/structurizr/structurizr.properties
```

## Properties

Structurizr properties are defined in the file `structurizr.properties`.

We set the `structurizr.autoRefreshInterval` to `2000` ms, to auto-reload the diagrams as the source changes ever 2 seconds.
