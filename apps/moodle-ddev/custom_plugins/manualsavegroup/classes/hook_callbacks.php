<?php

namespace local_manualsavegroup;

use core\hook\output\before_standard_head_html_generation;

defined('MOODLE_INTERNAL') || die();

class hook_callbacks {
    public static function before_head_html(before_standard_head_html_generation $hook): void {
        global $PAGE;
        if (strpos($PAGE->url->out(false), '/group/members.php') === false) {
            return;
        }
        $PAGE->requires->js_call_amd('local_manualsavegroup/manualsavegroup', 'init');
    }
}
