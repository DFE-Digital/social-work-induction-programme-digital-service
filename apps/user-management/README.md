# Social Worker Workforce - Early Career Framework

## Development

### Tooling and Package Management

#### Tool versions (`asdf`)
This repo has been setup to use [asdf](https://asdf-vm.com) to manage tooling versions. Follow their [Getting Started](https://asdf-vm.com/guide/getting-started.html) guide to install `asdf` on your machine.

#### Package manager (`pnpm`)
For package management, we use [pnpm](https://pnpm.io). Once you have installed `asdf`, you can setup `pnpm` by simply adding the required plugins and installing it:
```shell
asdf plugin add nodejs
asdf plugin add pnpm
asdf install
```

### Setup

- Install dependencies: `pnpm i`
- *Optionally*:
  - Install `nx` globally: `pnpm i -g nx`
  - **Or:** wherever the documentation refers to `nx ...`, just use `pnpx nx ...` instead.

### Tasks

#### Running tasks

Tasks can either be run workspace-wide with `nx run-many -t <taskName>`, or project specific with `nx run <project>:<taskName>`.
e.g.
- `nx run-many -t serve`
- `nx run frontend:build`

#### Task List

| Task Name | Description                               |
| --------- | ----------------------------------------- |
| `build`   | Builds the project(s)                     |
| `serve`   | Builds and runs a live development server |
| `test`    | Runs unit tests                           |
| `e2e`     | Runs E2E tests                            |
