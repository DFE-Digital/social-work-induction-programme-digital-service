start:
  docker compose up --build -d

stop:
  docker compose down

clean:
  docker compose down -v

install-moodle:
  . ./scripts/install-moodle.sh
