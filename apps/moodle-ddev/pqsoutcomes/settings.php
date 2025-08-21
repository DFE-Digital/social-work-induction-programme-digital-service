<?php
defined('MOODLE_INTERNAL') || die();
if ($ADMIN->fulltree) {
    $settings->add(new admin_setting_configcheckbox(
        'datafield_pqsoutcomes/enabled',
        get_string('enabled', 'datafield_pqsoutcomes'),
        get_string('enabled_desc', 'datafield_pqsoutcomes'),
        1
    ));
}