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

$THEME->parents_exclude_sheets = [
    'govuk' => ['govuk'],
];

// append govuk styles as a post process for theme styles to take precedence over boost
$THEME->csspostprocess = 'theme_govuk_swpdp_csspostprocess';

$THEME->rendererfactory = 'theme_overridden_renderer_factory';