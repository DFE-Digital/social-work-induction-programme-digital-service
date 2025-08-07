<?php
defined('MOODLE_INTERNAL') || die();

$callbacks = [
    [
        'hook'     => \core\hook\output\before_standard_head_html_generation::class,
        'callback' => [\local_manualsavegroup\hook_callbacks::class, 'before_head_html'],
        'priority' => 1000,
    ],
];
