# Use the [official Moodle PHP-apache image](https://github.com/moodlehq/moodle-php-apache) as a base
FROM moodlehq/moodle-php-apache:8.3-bookworm

USER www-data

# Clone the Moodle source code
RUN git clone -b MOODLE_405_STABLE --single-branch --depth=1 git://git.moodle.org/moodle.git && \
  mv -f ./moodle/* /var/www/html/ && rm -rf ./moodle

# Download and extract the theme
ARG GITHUB_RELEASE_URL="https://github.com/DFE-Digital/govuk-moodle-theme/releases/download/2024200800/govuk-moodle-theme.zip"
RUN curl -L -o /tmp/govuk-moodle-theme.zip "$GITHUB_RELEASE_URL" \
  && unzip /tmp/govuk-moodle-theme.zip -d /tmp/theme \
  && mv /tmp/theme/govuk/ /var/www/html/theme/ \
  && rm /tmp/govuk-moodle-theme.zip

# Copy custom config.php into the correct location
COPY config.php /var/www/html/config.php

WORKDIR /var/www/html

USER root

CMD ["apache2-foreground"]

EXPOSE 80
