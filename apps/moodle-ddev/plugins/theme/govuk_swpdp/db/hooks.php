<?php
defined('MOODLE_INTERNAL') || die();

$callbacks = [
    [
        'hook' => \core\hook\navigation\primary_extend::class,
        'callback' => [\theme_govuk_swpdp\hooks\primary_nav::class, 'extend'],
        'priority' => 100
    ],
];