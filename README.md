# Social Work Induction Programme (SWIP) digital service

This repository houses the core digital service for the Social Work Induction programme in children's social care. The digital service is based on [Moodle LMS](https://moodle.org).

## Local development setup
The `moodle-docker` image in this repository builds on the [official Moodle PHP-apache image](https://github.com/moodlehq/moodle-php-apache). There is a `compose.yml` that contains the necessary containers to run the service locally via `docker compose`.
For local development, a `development.env` file which is referenced in the `compose.yml` defines values for the various environment variables needed to run the service and it's accompanying database.

To simplify local development, a `justfile` contains various scripts for building, starting, and configuring the service.

---
### Windows Setup
You need to clone this repository into your user directory in the WSL directory. This is usually found in `\\wsl.localhost\Ubuntu\home\{USERNAME}`. You will now need to install [Just](https://github.com/casey/just) to access the justfile commands, alternatively you can run the docker commands.

#### Just Installation
In order to use the scripts in the `justfile`, you will need to install [just](https://github.com/casey/just) in your development environment.
It's recommended to install [Homebrew](https://brew.sh/), a package management system. To do this open the WSL terminal and run `/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"`.

Followed by these commands:
```
test -d /home/linuxbrew/.linuxbrew && eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)"
echo "eval \"\$($(brew --prefix)/bin/brew shellenv)\"" >> ~/.bashrc
```

To finalise the install of Brew you need to run the  `brew install gcc`. Once this command has successfully ran you can now install Just by running `brew install just`. Once the command finished you can run `just --version` to confirm that Just is installed and working.

---

### Running the service
Once you have `just` installed you will be able to run the commands in the `justfile`.
- `just start` to start Moodle.
- `just stop` to stop Moodle.
- `just clean` will stop the service and prune the volumes from docker.
- `just install-moodle` to install/configure moodle. This is only needed on the first run.
- `just build` will force rebuild the moodle-docker image.

### Moodle Configuration

When running the service via docker compose, the `config.php` file in `moodle-docker` is mapped to Moodle's own `config.php` in the webroot of the container. The documentation for this configuration can be found [here](https://docs.moodle.org/405/en/Configuration_file).
