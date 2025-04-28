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

$host = $_SERVER['HTTP_HOST'] ?? getenv('MOODLE_DOCKER_WEB_HOST') ?? 'localhost';
if (empty($_SERVER['HTTP_HOST'])) {
  $_SERVER['HTTP_HOST'] = $host;
}
$httpOrS = '';
if (getenv('MOODLE_DOCKER_SSL_TERMINATION') === 'true') {
  $CFG->sslproxy = true;
  $httpOrS = 's';
}
$CFG->wwwroot = "http{$httpOrS}://{$host}";
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

$CFG->theme = 'govuk';

// The Moodle instances should NOT run their own cron jobs
$CFG->cronclionly = true;

require_once(__DIR__ . '/lib/setup.php');

// There is no php closing tag in this file,
// it is intentional because it prevents trailing whitespace problems!
