start:
  docker compose up --build -d

build:
  docker compose build --no-cache --parallel

stop:
  docker compose down

clean:
  docker compose down -v

install-moodle:
  docker exec --env-file development.env -it --user www-data moodle-web bash -c 'php /var/www/html/admin/cli/install_database.php --agree-license --fullname="${MOODLE_SITE_FULLNAME}" --shortname="${MOODLE_SITE_SHORTNAME}" --adminpass="${MOODLE_ADMIN_PASSWORD}" --adminemail="${MOODLE_ADMIN_EMAIL}"'
