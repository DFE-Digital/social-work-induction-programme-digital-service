# Use the official PHP 8.3 image as a base
FROM php:8.3-apache

# Install necessary extensions, Apache mod_rewrite, Composer, PostgreSQL extension, and OPcache
RUN apt-get update && \
  apt-get install -y --no-install-recommends unzip git curl libzip-dev libjpeg-dev libpng-dev \
  libfreetype6-dev libicu-dev libxml2-dev libpq-dev && \
  docker-php-ext-configure gd --with-freetype --with-jpeg && \
  docker-php-ext-install mysqli zip gd intl soap exif pgsql pdo_pgsql opcache && \
  apt-get clean && \
  rm -rf /var/lib/apt/lists/* && \
  git clone git://git.moodle.org/moodle.git && cd moodle && \
  git branch --track MOODLE_405_STABLE origin/MOODLE_405_STABLE && \
  git checkout MOODLE_405_STABLE && \
  cp -rf ./* /var/www/html/

# Set PHP settings for Moodle: max_input_vars and OPcache
RUN echo "max_input_vars=5000" >> /usr/local/etc/php/conf.d/docker-php-moodle.ini && \
  echo "opcache.enable=1" >> /usr/local/etc/php/conf.d/docker-php-opcache.ini && \
  echo "opcache.enable_cli=1" >> /usr/local/etc/php/conf.d/docker-php-opcache.ini && \
  echo "opcache.memory_consumption=128" >> /usr/local/etc/php/conf.d/docker-php-opcache.ini && \
  echo "opcache.interned_strings_buffer=8" >> /usr/local/etc/php/conf.d/docker-php-opcache.ini && \
  echo "opcache.max_accelerated_files=10000" >> /usr/local/etc/php/conf.d/docker-php-opcache.ini && \
  echo "opcache.revalidate_freq=60" >> /usr/local/etc/php/conf.d/docker-php-opcache.ini && \
  echo "opcache.validate_timestamps=1" >> /usr/local/etc/php/conf.d/docker-php-opcache.ini

# Create moodledata directory
RUN mkdir -p /var/www/moodledata

# Set working directory
WORKDIR /var/www/html

## Theme install

# Set the GitHub release URL (modify this based on the actual repository)
ARG GITHUB_RELEASE_URL="https://github.com/DFE-Digital/govuk-moodle-theme/releases/download/2024200800/govuk-moodle-theme-2024200800.zip"

# Download and extract the theme
RUN curl -L -o /tmp/govuk-moodle-theme-2024200800.zip "$GITHUB_RELEASE_URL" \
    && unzip /tmp/govuk-moodle-theme-2024200800.zip -d /tmp/theme \
    && mv /tmp/theme/govuk-moodle-theme-2024200800/govuk/ /var/www/html/theme/ \
    && rm /tmp/govuk-moodle-theme-2024200800.zip

# Set the correct permissions
RUN chown -R www-data:www-data /var/www/ && chmod -R 755 /var/www

# Enable Apache rewrite module (if needed for Moodle)
RUN a2enmod rewrite

# Restart Apache
CMD ["apache2-foreground"]

## End theme install 

# Expose port 80
EXPOSE 80
