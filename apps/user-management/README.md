# Social Worker Workforce - Early Career Framework

## Development

### Coding Conventions & Standards
#### CSharpier
We apply coding standards using [CSharpier](https://csharpier.com/) and `.editorconfig`. In order to apply the formatting on save you can install the extention for your chosen IDE [here](https://csharpier.com/docs/Editors), and follow the instructions in the extension overview.

#### Extension Links
- [Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=csharpier.CSharpier) - `Configure Reformat with CSharpier on Save under Tools | Options | CSharpier | General`
- [JetBrains Rider](https://plugins.jetbrains.com/plugin/18243-csharpier) - `Configure CSharpier to Run on Save under Preferences/Settings | Tools | CSharpier`

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
