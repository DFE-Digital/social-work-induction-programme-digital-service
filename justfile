start:
  docker compose up --build -d

build:
  docker compose build --no-cache --parallel

stop:
  docker compose down

clean:
  docker compose down -v

install-moodle:
  . ./scripts/install-moodle.sh
