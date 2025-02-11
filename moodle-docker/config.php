<?php  // Moodle configuration file

unset($CFG);
global $CFG;
$CFG = new stdClass();

$CFG->dbtype = getenv('MOODLE_DB_TYPE');
$CFG->dblibrary = 'native';
$CFG->dbhost = getenv('MOODLE_DB_HOST');
$CFG->dbname = getenv('POSTGRES_DB');
$CFG->dbuser = getenv('POSTGRES_USER');
$CFG->dbpass = getenv('POSTGRES_PASSWORD');
$CFG->prefix = getenv('MOODLE_DB_PREFIX');
$CFG->dboptions = [
  'dbpersist' => 0,
  'dbport' => 5432,
  'dbsocket' => '',
];

if (empty($_SERVER['HTTP_HOST'])) {
  $_SERVER['HTTP_HOST'] = 'localhost';
}
$host = 'localhost';
if (!empty(getenv('MOODLE_DOCKER_WEB_HOST'))) {
  $host = getenv('MOODLE_DOCKER_WEB_HOST');
}
$CFG->wwwroot = "http://{$host}";
$port = getenv('MOODLE_DOCKER_WEB_PORT');
if (!empty($port)) {
  // Extract port in case the format is bind_ip:port.
  $parts = explode(':', $port);
  $port = end($parts);
  if ((string) (int) $port === (string) $port) { // Only if it's int value.
    $CFG->wwwroot .= ":{$port}";
  }
}

$CFG->dataroot = '/var/www/moodledata';
$CFG->admin = getenv('MOODLE_ADMIN_USER');

$CFG->directorypermissions = 02777;

# Uncomment to set the theme
#$CFG->theme = 'govuk';

require_once(__DIR__ . '/lib/setup.php');

// There is no php closing tag in this file,
// it is intentional because it prevents trailing whitespace problems!
