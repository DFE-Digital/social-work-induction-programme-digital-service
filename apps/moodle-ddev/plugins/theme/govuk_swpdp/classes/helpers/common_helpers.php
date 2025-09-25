<?php
namespace theme_govuk_swpdp\helpers;

defined('MOODLE_INTERNAL') || die();

/**
 * Customise the primary navigation
 */
class common_helpers {

    private static array $cache = [];

    public static function is_staff_like(int $userid): bool {
        global $CFG;
        
        if (array_key_exists($userid, self::$cache)) {
            return self::$cache[$userid];
        }

        // system capabilities checks
        $system = \context_system::instance();
        if (is_siteadmin($userid) || has_any_capability([
            'moodle/site:config',
            'moodle/course:create',
            'moodle/site:manageblocks'
        ], $system, $userid)) {
            return self::$cache[$userid] = true;
        }
        
        // staff capabilities checks
        require_once($CFG->libdir . '/enrollib.php');
        $courses = enrol_get_users_courses($userid, true, 'id', 'sortorder ASC');
        if (!$courses) { 
            return self::$cache[$userid] = false; 
        }

        $staffcapabilities = [
            'moodle/course:managegroups',
            'moodle/competency:usercompetencyview'
        ];

        foreach ($courses as $course) {
            $context = \context_course::instance($course->id);
            if (has_any_capability($staffcapabilities, $context, $userid)) {
                return self::$cache[$userid] = true;
            }
        }

        return self::$cache[$userid] = false;
    }
}