APACHE_CONF="/etc/apache2/sites-available/000-default.conf"

# Update DocumentRoot to point to /var/www/html/public
if sed -n '/<VirtualHost \*:80>/,/<\/VirtualHost>/ { /DocumentRoot/p }' "$APACHE_CONF" | grep -q .; then
    # Replace existing DocumentRoot (preserve indentation)
    sed -i '/<VirtualHost \*:80>/,/<\/VirtualHost>/ s|^\([[:space:]]*\)DocumentRoot .*|\1DocumentRoot /var/www/html/public|' "$APACHE_CONF"
else
    # Add DocumentRoot after ServerAdmin line (BSD/GNU compatible)
    sed -i '' '/<VirtualHost \*:80>/,/<\/VirtualHost>/ {
        /ServerAdmin/ a\
        DocumentRoot /var/www/html/public
    }' "$APACHE_CONF"
fi

# Update DirectoryIndex inside the VirtualHost block
if sed -n '/<VirtualHost \*:80>/,/<\/VirtualHost>/ { /DirectoryIndex/p }' "$APACHE_CONF" | grep -q .; then
    # Replace existing DirectoryIndex (preserve indentation)
    sed -i '/<VirtualHost \*:80>/,/<\/VirtualHost>/ s|^\([[:space:]]*\)DirectoryIndex .*|\1DirectoryIndex index.html|' "$APACHE_CONF"
else
    # Add DirectoryIndex after DocumentRoot line (BSD/GNU compatible)
    sed -i -e '/<VirtualHost \*:80>/,/<\/VirtualHost>/ {' \
        -e '/DocumentRoot/ a\'$'\n''        DirectoryIndex index.html' \
        -e '}' "$APACHE_CONF"
fi
cat "$APACHE_CONF"
