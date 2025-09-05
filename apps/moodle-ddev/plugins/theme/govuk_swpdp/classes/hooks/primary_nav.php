<?php
namespace theme_govuk_swpdp\hooks;

defined('MOODLE_INTERNAL') || die();

use core\hook\navigation\primary_extend;
use context_system;
use moodle_url;
use context_course;

/**
 * Customise the primary navigation
 */
class primary_nav {
    public static function extend(primary_extend $hook): void {
        global $USER,  $PAGE, $CFG, $COURSE;

        if (during_initial_install()) { return; }

        if (!isloggedin() || isguestuser()) {
            return;
        }

        $conf = get_config('theme_govuk_swpdp');
        if (empty($conf->enablelearnercustomnav)) { return; }

        if (self::is_staff_like($USER->id)) { return; } 

        $primary_view = $hook->get_primaryview();

        foreach (['home', 'myhome', 'dashboard', 'mycourses', 'courses'] as $key) {
            if ($node = $primary_view->get($key)) {
                $node->remove();
            }
        }

        $dashboard = $primary_view->add(
            get_string('myhome'),
            new moodle_url('/my/'),
            \navigation_node::TYPE_CUSTOM,
            null,
            'custom-dashboard'
        );

        require_once($CFG->libdir . '/enrollib.php');
        $courses = enrol_get_my_courses('id', 'sortorder ASC', 0, false);

        if (count($courses) === 1) {
            $course = reset($courses);
            $singlecourseid = (int)$course->id;
            $portfolio = $primary_view->add(
                get_string('myportfolio', 'theme_govuk_swpdp'),
                new moodle_url('/course/view.php', ['id' => $course->id]),
                \navigation_node::TYPE_CUSTOM,
                null,
                'custom-portfolio'
            );
        } else {
            $portfolio = $primary_view->add(
                get_string('myportfolio', 'theme_govuk_swpdp'),
                new moodle_url('/my/courses.php'),
                \navigation_node::TYPE_CUSTOM,
                null,
                'custom-portfolio'
            );
        }

        $primary_view->add(
            get_string('learningmaterials', 'theme_govuk_swpdp'),
            '#',
            \navigation_node::TYPE_CUSTOM,
            null,
            'custom-learning'
        );

        // active tab handling
        if ($PAGE->url->compare(new moodle_url('/my/'), URL_MATCH_BASE)) {
            $dashboard->make_active();
        }

        if ($singlecourseid) {
            if (!empty($COURSE->id) && $COURSE->id == $singlecourseid) {
                $portfolio->make_active();
            } else {
                $course_context = context_course::instance($singlecourseid, IGNORE_MISSING);
                if ($course_context && !empty($PAGE->context->path) &&
                    strpos($PAGE->context->path, '/' . $course_context->id . '/') !== false) {
                    $portfolio->make_active();
                }
            }
        } else {
            // fallback and make active on /my/courses.php
            if ($PAGE->url->compare(new moodle_url('/my/courses.php'), URL_MATCH_BASE)) {
                $portfolio->make_active();
            }
        }

    }

    private static function is_staff_like(int $userid): bool {
        global $CFG, $DB;

        // system capabilities checks
        $system = \context_system::instance();
        if (is_siteadmin($userid) || has_any_capability([
            'moodle/site:config',
            'moodle/course:create',
            'moodle/site:manageblocks'
        ], $system, $userid)) {
            return true;
        }
        
        // assessor capabilities checks
        require_once($CFG->libdir . '/enrollib.php');
        $courses = enrol_get_users_courses($userid, true, 'id', 'sortorder ASC');
        if (!$courses) { return false; }

        $assessorcaps = [
            'moodle/competency:usercompetencyview'
        ];

        foreach ($courses as $course) {
            $context = \context_course::instance($course->id, IGNORE_MISSING);
            if (has_any_capability($assessorcaps, $context, $userid)) {
                return true;
            }
        }

        return false;
    }
}