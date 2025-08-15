<?php
namespace availability_save;

defined('MOODLE_INTERNAL') || die();

class frontend extends \core_availability\frontend {
    protected function get_javascript_strings() {
        return [
            'label_selectactivity',
            'error_noactivity',
            'error_sameactivity'
        ];
    }

    protected function get_javascript_init_params($course, \cm_info $cm = null, \section_info $section = null) {
        // Provide a list of Database activities in this course.
        $modinfo = get_fast_modinfo($course);
        $databases = [];
        $targetcmid = $cm ? $cm->id : 0;

        foreach ($modinfo->cms as $cmid => $cminfo) {
            if ($cminfo->modname === 'data' && !$cminfo->deletioninprogress) {
                if ($targetcmid && $cmid === $targetcmid) {
                    continue;
                }

                $databases[] = [
                    'cmid' => $cmid,
                    'name' => format_string($cminfo->name, true, ['context' => $cminfo->context]),
                    'section' => $cminfo->sectionnum,
                ];
            }
        }
        return ['databases' => $databases, 'targetcmid' => $targetcmid];
    }

    protected function allow_add($course, \cm_info $cm = null, \section_info $section = null) {
        return true;
    }
}
