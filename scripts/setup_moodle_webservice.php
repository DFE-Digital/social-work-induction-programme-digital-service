<?php
// Moodle bootstrap
define('CLI_SCRIPT', true);
require(__DIR__ . '/config.php');
require_once($CFG->libdir . '/clilib.php');
require_once($CFG->libdir . '/accesslib.php');
require_once($CFG->dirroot . '/user/lib.php');
require_once($CFG->dirroot . '/webservice/lib.php');

[$options, $unrecognized] = cli_get_params([
    'help' => false,
    'username' => null,
    'password' => null,
    'email' => null,
    'servicename' => null,
    'token' => null,
    'rolename' => 'Web Service Role',
    'roleshortname' => 'webservice_role',
    'capabilities' => null,
], [
    'h' => 'help'
]);

if ($options['help']) {
    echo "Usage: php setup_webservice.php --username=USER --password=PASS --email=EMAIL --servicename=NAME [--token=TOKEN] [--rolename=NAME] [--capabilities=cap1,cap2,...]\n";
    exit(0);
}

foreach (['username', 'password', 'email', 'servicename'] as $required) {
    if (empty($options[$required])) {
        cli_error("Missing required option --$required");
    }
}

$username = $options['username'];
$password = $options['password'];
$email = $options['email'];
$servicename = $options['servicename'];
$tokenParam = $options['token'];
$rolename = $options['rolename'];
$roleshortname = $options['roleshortname'];
$capabilities = !empty($options['capabilities']) ? explode(',', $options['capabilities']) : [
    'webservice/rest:use',
    'moodle/user:view',
    'moodle/user:create',
    'moodle/user:update',
    'moodle/course:viewhiddencourses',
    'moodle/course:update',
    'moodle/course:view'
];

// Ensure the role exists
$roleid = $DB->get_field('role', 'id', ['shortname' => $roleshortname], IGNORE_MULTIPLE);
if (!$roleid) {
    $roleid = create_role($rolename, $roleshortname, 'Role for external web service access');
    echo "Created role: $rolename\n";
} else {
    echo "Role already exists: $rolename\n";
}

// Ensure user exists
$user = $DB->get_record('user', ['username' => $username]);
if (!$user) {
    $user = new stdClass();
    $user->username = $username;
    $user->password = hash_internal_user_password($password);
    $user->email = $email;
    $user->firstname = 'Web';
    $user->lastname = 'Service';
    $user->auth = 'manual';
    $user->confirmed = 1;
    $user->mnethostid = $CFG->mnet_localhost_id;
    
    // Create the user in the database
    $userid = user_create_user($user, false);
    $user = $DB->get_record('user', ['id' => $userid]);
    echo "Created user: $username\n";

    // Update the 'auth' field to 'webservice'
    $user->auth = 'webservice'; 
    $DB->update_record('user', $user); 
    user_update_user($user);

    echo "User updated: {$user->username}, Auth: {$user->auth}\n";
} else {
    echo "User already exists: $username\n";
}

// Assign role to user at system level
$context = context_system::instance();
if (!user_has_role_assignment($user->id, $roleid, $context->id)) {
    role_assign($roleid, $user->id, $context->id);
    echo "Assigned role '$rolename' to user '$username'\n";
} else {
    echo "User already has role '$rolename'\n";
}

// Add capabilities to the role
foreach ($capabilities as $cap) {
    $exists = $DB->get_record('capabilities', ['name' => $cap]);
    if (!$exists) {
        echo "⚠️ Capability '$cap' not found in Moodle. Skipping.\n";
        continue;
    }

    foreach ($capabilities as $cap) {
        $exists = $DB->get_record('capabilities', ['name' => $cap]);
        if (!$exists) {
            echo "⚠️ Capability '$cap' not found in Moodle. Skipping.\n";
            continue;
        }
    
        $alreadyassigned = $DB->record_exists('role_capabilities', [
            'capability' => $cap,
            'roleid' => $roleid,
            'contextid' => $context->id,
        ]);
    
        if (!$alreadyassigned) {
            assign_capability($cap, CAP_ALLOW, $roleid, $context->id);
            echo "✅ Assigned capability: $cap\n";
        } else {
            echo "🔁 Capability already assigned: $cap\n";
        }
    }    
}

// Ensure external service exists
$service = $DB->get_record('external_services', ['name' => $servicename]);
if (!$service) {
    $service = new stdClass();
    $service->name = $servicename;
    $service->enabled = 1;
    $service->shortname = preg_replace('/[^a-zA-Z0-9_]/', '_', strtolower($servicename));
    $service->restrictedusers = 1;
    $service->requiredcapability = '';
    $service->component = '';
    $service->timecreated = time(); 
    $service->timemodified = time();
    $serviceid = $DB->insert_record('external_services', $service);
    echo "Created web service: $servicename\n";
} else {
    $serviceid = $service->id;
    echo "Web service already exists: $servicename\n";
}

// Link user to the service
$exists = $DB->record_exists('external_services_users', ['externalserviceid' => $serviceid, 'userid' => $user->id]);
if (!$exists) {
    $svcuser = new stdClass();
    $svcuser->externalserviceid = $serviceid;
    $svcuser->userid = $user->id;
    $svcuser->timecreated = time();
    $svcuser->creatorid = $USER->id;
    $DB->insert_record('external_services_users', $svcuser);
    echo "Linked user to web service\n";
} else {
    echo "User already linked to web service\n";
}

// Generate token
$token = $DB->get_record('external_tokens', [
    'userid' => $user->id,
    'externalserviceid' => $serviceid,
    'tokentype' => EXTERNAL_TOKEN_PERMANENT
]);

if (!$token) {
    $token = new stdClass();
    # Support provided token from command line or generated token 
    $token->token = $tokenParam ?? md5(uniqid(rand(), 1));
    $token->userid = $user->id;
    $token->externalserviceid = $serviceid;
    $token->contextid = $context->id;
    $token->creatorid = $USER->id;
    $token->timecreated = time();
    $token->validuntil = 0;
    $token->iprestriction = '';
    $token->sid = '';
    $token->tokentype = EXTERNAL_TOKEN_PERMANENT;
    $DB->insert_record('external_tokens', $token);
    echo "Created token: {$token->token}\n";
} else {
    echo "Token already exists: {$token->token}\n";
}

// List of Moodle web service functions you want to allow
$functions = [
#    'core_user_get_users',
    'core_course_get_courses',
];

foreach ($functions as $functionname) {
    // Check if the function exists in the external_functions table
    $functionexists = $DB->record_exists('external_functions', ['name' => $functionname]);
    if (!$functionexists) {
        echo "⚠️ Function '$functionname' does not exist in Moodle. Skipping.\n";
        continue;
    }

    // Check if it's already assigned to the service
    $alreadyassigned = $DB->record_exists('external_services_functions', [
        'externalserviceid' => $serviceid,
        'functionname' => $functionname
    ]);

    if (!$alreadyassigned) {
        $servicefunction = new stdClass();
        $servicefunction->externalserviceid = $serviceid;
        $servicefunction->functionname = $functionname;
        $DB->insert_record('external_services_functions', $servicefunction);
        echo "✅ Assigned function: $functionname\n";
    } else {
        echo "🔁 Function already assigned: $functionname\n";
    }
}