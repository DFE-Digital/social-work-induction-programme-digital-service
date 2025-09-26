<?php
defined('MOODLE_INTERNAL') || die();

$THEME->name = 'govuk_swpdp';

/**
 * To inherit both govuk and boost styles, both themes need listing as parents.
 * The order of the parents matters. For this theme to inherit
 * govuk templates that overwrite boost templates,
 * govuk must be listed first
 */
$THEME->parents = ['govuk','boost'];

$THEME->scss = function($theme) {
    return theme_govuk_swpdp_get_main_scss_content($theme);
};
$THEME->prescsscallback   = 'theme_govuk_swpdp_get_pre_scss';
$THEME->extrascsscallback = 'theme_govuk_swpdp_get_extra_scss';

$THEME->rendererfactory = 'theme_overridden_renderer_factory';