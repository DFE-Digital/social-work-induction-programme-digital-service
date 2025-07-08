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
# Store sessions in the database - the file session handler doesn't like the default Azure File
# Share permissions and gives: 
# Warning: session_start(): Session data file is not created by your uid in /var/www/html/public/lib/classes/session/handler.php on line 39
# Warning: session_start(): Failed to read session data: files (path: /var/www/moodledata/sessions) in /var/www/html/public/lib/classes/session/handler.php on line 39
$CFG->session_handler_class = '\core\session\database';
$CFG->session_database_acquire_lock_timeout = 120;

# Get host name from Front Door forward header first, then fallback
$host = $_SERVER['HTTP_X_FORWARDED_HOST'] ?? $_SERVER['HTTP_HOST'] ?? getenv('MOODLE_DOCKER_WEB_HOST') ?? 'localhost';
$_SERVER['HTTP_HOST'] = $host;
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

if (getenv('MOODLE_SWITCH_OFF_GOVUK_THEMING') !== 'true') {
  $CFG->theme = 'govuk'; 
}
if (getenv('MOODLE_SWITCH_OFF_OAUTH') === 'true') {
  $CFG->auth = 'manual'; 
}

// The Moodle instances should NOT run their own cron jobs
$CFG->cronclionly = true;

require_once(__DIR__ . '/lib/setup.php');

// There is no php closing tag in this file,
// it is intentional because it prevents trailing whitespace problems!
