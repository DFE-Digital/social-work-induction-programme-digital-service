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
$THEME->sheets = ['govuk_swpdp'];

$THEME->rendererfactory = 'theme_overridden_renderer_factory';