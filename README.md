# Social Work Induction Programme (SWIP) digital service

This repository houses the core digital service for the Social Work Induction programme in children's social care. The digital service is based on Moodle LMS.

## Local development setup
To install Moodle LMS and run it locally the recommended tool is DDEV. DDEV is an open-source tool for launching containerised local web development environments quickly and easily, without requiring extensive Docker knowledge. 

To install DDEV, follow the official guide for your operating system: [Installing DDEV](https://ddev.readthedocs.io/en/stable/users/install/).

Once DDEV is successfully installed, follow the DDEV quickstart guide for installing and running a Moodle site: [Moodle quickstart](https://ddev.readthedocs.io/en/stable/users/quickstart/#moodle).

Note: The LTS version of Moodle will be installed.

---
### Windows Setup
You need to clone this repository into your user directory in the WSL directory (follow the DDEV installation guide above). This is usually found in `\\wsl.localhost\Ubuntu\home\{USERNAME}`. You will now need to install [Just](https://github.com/casey/just) to access the justfile commands, alternatively you can run the docker commands.

#### Just Installation
It's recommended to install [Homebrew](https://brew.sh/), a package management system. To do this open the WSL terminal and run `/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"`. 

Followed by these commands: 
```
test -d /home/linuxbrew/.linuxbrew && eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)"
echo "eval \"\$($(brew --prefix)/bin/brew shellenv)\"" >> ~/.bashrc
```

To finalise the install of Brew you need to run the  `brew install gcc`. Once this command has successfully ran you can now install Just by running `brew install just`. Once the command finished you can run `just --version` to confirm that Just is installed and working.

---

### Running Moodle
Once you have Just installed you will be able to run the commands in the justfile.
- `just start` to start Moodle
- `just stop` to stop Moodle
- `just install-moodle` to install/configure moodle. This is only needed on the first run