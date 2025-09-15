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
