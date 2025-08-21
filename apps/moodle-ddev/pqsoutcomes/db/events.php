<?php
defined('MOODLE_INTERNAL') || die();

$observers = [
    [
        'eventname'   => '\core\event\course_module_completion_updated',
        'callback'    => '\datafield_pqsoutcomes\observer::on_cm_completion',
        'includefile' => '/mod/data/field/pqsoutcomes/classes/observer.php',
        'internal'    => false,
        'priority'    => 9999,
    ],
];