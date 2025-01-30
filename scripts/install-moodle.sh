#!/bin/bash

docker exec -it --user www-data moodle-web php /var/www/html/admin/cli/install.php --non-interactive --agree-license --wwwroot=http://localhost:8080 --dbtype=pgsql --dbhost=postgres --dbport=5432 --dbname=moodle --dbuser=moodle_user --dbpass=moodle_password --fullname="Moodle Demo" --shortname=Demo --adminpass=password
