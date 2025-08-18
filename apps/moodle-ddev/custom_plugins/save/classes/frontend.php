<?php
namespace availability_save;
defined('MOODLE_INTERNAL') || die();

class frontend extends \core_availability\frontend {
    protected function get_javascript_strings() {
        return ['label_selectactivity','error_noactivity','error_sameactivity','title'];
    }

    protected function get_javascript_init_params($course, \cm_info $cm = null, \section_info $section = null) {
        $modinfo = get_fast_modinfo($course);
        $databases = [];
        foreach ($modinfo->cms as $cmid => $cminfo) {
            if ($cminfo->deletioninprogress) { continue; }
            if ($cminfo->modname !== 'data') { continue; }
            if ($cm && $cmid === $cm->id) { continue; } // don't allow watching itself
            $databases[] = ['cmid' => (int)$cmid, 'name' => (string)$cminfo->name];
        }

        return [[
            'databases'  => $databases,
            'targetcmid' => $cm ? (int)$cm->id : 0,
        ]];
    }

    protected function allow_add($course, \cm_info $cm = null, \section_info $section = null) {
        $modinfo = get_fast_modinfo($course);
        foreach ($modinfo->cms as $cminfo) {
            if (!$cminfo->deletioninprogress && $cminfo->modname === 'data') { return true; }
        }
        return false;
    }
}