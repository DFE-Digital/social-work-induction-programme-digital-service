<?php 
namespace theme_govuk_swpdp\hooks;

defined('MOODLE_INTERNAL') || die();

use core\hook\navigation\primary_extend;
use context_course;
use moodle_url;

class primary_nav {
    public static function extend(primary_extend $hook): void {
        global $USER, $PAGE, $COURSE, $DB, $CFG;

        if (!isloggedin() || isguestuser()) return;

        if (self::is_staff_like($USER->id)) {
            // Do not change nav for admins or staff
            return;
        }

        $primary_view = $hook->get_primaryview();

        // Remove default nodes
        foreach (['home','myhome','dashboard','mycourses','courses'] as $key) {
            if ($node = $primary_view->get($key)) $node->remove();
        }

        // Dashboard link
        $dashboard = $primary_view->add(
            get_string('myhome'),
            new moodle_url('/my/'),
            \navigation_node::TYPE_CUSTOM,
            null,
            'custom-dashboard'
        );

        // Single course handling
        $courses = enrol_get_my_courses('id', 'sortorder ASC', 0, false);
        $singlecourseid = count($courses) === 1 ? reset($courses)->id : null;

        if ($singlecourseid) {
            $portfolio_url = new moodle_url('/course/view.php', ['id' => $singlecourseid]);

            // Check if a portfolio section is configured
            if ($record = $DB->get_record('theme_govuk_swpdp_portfolio', ['courseid' => $singlecourseid])) {
                $portfolio_url = new moodle_url('/course/section.php', ['id' => $record->sectionid]);
            }

            $portfolio = $primary_view->add(
                get_string('myportfolio', 'theme_govuk_swpdp'),
                $portfolio_url,
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

        // Learning materials placeholder
        $primary_view->add(
            get_string('learningmaterials', 'theme_govuk_swpdp'),
            '#',
            \navigation_node::TYPE_CUSTOM,
            null,
            'custom-learning'
        );

        // Active tab logic
        if ($PAGE->url->compare(new moodle_url('/my/'), URL_MATCH_BASE)) {
            $dashboard->make_active();
        }
        if ($singlecourseid) {
            $record = $DB->get_record('theme_govuk_swpdp_portfolio', ['courseid' => $singlecourseid]);
            if ($record && $PAGE->url->compare(new moodle_url('/course/section.php', ['id' => $record->sectionid]), URL_MATCH_BASE)) {
                $portfolio->make_active();
            } else if (!empty($COURSE->id) && $COURSE->id == $singlecourseid) {
                $portfolio->make_active();
            }
        }
    }

    /**
     * Detect whether a user is "staff-like" and should not see learner nav
     */
    private static function is_staff_like(int $userid): bool {
        global $CFG, $DB;

        $system = \context_system::instance();
        if (is_siteadmin($userid) || has_any_capability([
            'moodle/site:config',
            'moodle/course:create',
            'moodle/site:manageblocks'
        ], $system, $userid)) {
            return true;
        }

        require_once($CFG->libdir . '/enrollib.php');
        $courses = enrol_get_users_courses($userid, true, 'id', 'sortorder ASC');
        if (!$courses) { 
            return false; 
        }

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
