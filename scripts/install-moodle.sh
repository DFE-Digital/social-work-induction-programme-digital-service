#!/bin/bash

docker exec -it --user www-data moodle-web php /var/www/html/admin/cli/install_database.php --agree-license --fullname="Social Work Induction Programme" --shortname=SWIP --adminpass=password --adminemail=admin@email.com
