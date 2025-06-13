<?php
// Define Moodle local plugin version for Application Insights
defined('MOODLE_INTERNAL') || die();
$plugin->component = 'local_appinsights';
$plugin->version   = 2025060100; // YYYYMMDDXX - set to a date after Moodle version
$plugin->requires  = 2024100705; // Moodle 3.11 requires this or later, adjust for correct Moodle version
$plugin->maturity  = MATURITY_STABLE;
$plugin->release   = '1.0';