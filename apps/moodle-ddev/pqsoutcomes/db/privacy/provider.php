<?php
namespace datafield_pqsoutcomes;
defined('MOODLE_INTERNAL') || die();

class provider implements \core_privacy\local\metadata\null_provider {
    public static function get_reason(): string {
        return get_string('privacy:metadata', 'datafield_pqsoutcomes');
    }
}