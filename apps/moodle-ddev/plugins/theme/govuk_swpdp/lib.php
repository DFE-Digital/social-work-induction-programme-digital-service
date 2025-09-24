<?php
defined('MOODLE_INTERNAL') || die();

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

function theme_govuk_swpdp_get_main_scss_content($theme): string {
    $parentscss = theme_govuk_get_main_scss_content($theme);
    $themescss = @file_get_contents(__DIR__ . '/scss/govuk_swpdp.scss') ?: '';
    return $parentscss . "\n" . $themescss;
}
function theme_govuk_swpdp_get_pre_scss($theme): string {
    return theme_govuk_get_pre_scss($theme) . "\n" .
           (@file_get_contents(__DIR__ . '/scss/pre.scss') ?: '');
}
function theme_govuk_swpdp_get_extra_scss($theme): string {
    return theme_govuk_get_extra_scss($theme) . "\n" .
           (@file_get_contents(__DIR__ . '/scss/post.scss') ?: '');
}
