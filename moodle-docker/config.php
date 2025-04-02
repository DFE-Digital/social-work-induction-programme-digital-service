<?php  // Moodle configuration file

unset($CFG);
global $CFG;
$CFG = new stdClass();

$CFG->dbtype = getenv('MOODLE_DB_TYPE');
$CFG->dblibrary = 'native';
$CFG->dbhost = getenv('MOODLE_DB_HOST');
print_r($CFG->dbhost);
$result = dns_get_record($CFG->dbhost);
print_r($result);
$connection = @fsockopen($CFG->dbhost, 5432);
if (is_resource($connection))
{
    echo '<h2>' . $CFG->dbhost . ':' . 5432 . ' ' . '(' . getservbyport(5432, 'tcp') . ') is open.</h2>' . "\n";

    fclose($connection);
}
else
{
    echo '<h2>' . $CFG->dbhost . ':' . 5432 . ' is not responding.</h2>' . "\n";
}

$CFG->dbname = getenv('POSTGRES_DB');
print_r($CFG->dbname);
$CFG->dbuser = getenv('POSTGRES_USER');
print_r($CFG->dbuser);
$CFG->dbpass = getenv('POSTGRES_PASSWORD');
print_r($CFG->dbpass);
$CFG->prefix = getenv('MOODLE_DB_PREFIX');
print_r($CFG->prefix);
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

$CFG->theme = 'govuk';

require_once(__DIR__ . '/lib/setup.php');

// There is no php closing tag in this file,
// it is intentional because it prevents trailing whitespace problems!
