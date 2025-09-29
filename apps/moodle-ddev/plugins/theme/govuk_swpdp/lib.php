<?php
defined('MOODLE_INTERNAL') || die();

/**
 * Add the govuk css to the styles to ensure its precedence over boost styles
 */
function theme_govuk_swpdp_csspostprocess(string $css, theme_config $theme): string {
    global $CFG;

    $govukpath = $CFG->dirroot . '/theme/govuk/style/govuk.css';
    if (is_readable($govukpath)) {
        $govukcss = file_get_contents($govukpath);
        $css .=  $govukcss;
    }

    return $css;
}

/**
 * Add "Portfolio Section" to the course settings menu.
 */
function theme_govuk_swpdp_extend_settings_navigation($settingsnav, $context) {
    if ($context->contextlevel !== CONTEXT_COURSE) {
        return;
    }

    $courseid = $context->instanceid;

    if (!has_capability('moodle/course:update', $context)) {
        return;
    }

    $url = new moodle_url('/theme/govuk_swpdp/course_portfolio_settings.php', ['id' => $courseid]);
    $settingsnav->add(get_string('myportfolio', 'theme_govuk_swpdp'), $url);
}