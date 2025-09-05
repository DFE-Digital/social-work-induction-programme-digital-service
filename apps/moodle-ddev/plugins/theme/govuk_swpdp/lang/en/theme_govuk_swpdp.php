<?php
// This file is part of Moodle - http://moodle.org/
//
// Moodle is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Moodle is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Moodle.  If not, see <http://www.gnu.org/licenses/>.

/**
 * Theme functions.
 *
 * @package    theme_govuk_swpdp
 * @copyright  Copyright (c) Crown Copyright (Department for Education)
 * @license    https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/ Open Government Licence v3.0
 */

// This line protects the file from being accessed by a URL directly.
defined('MOODLE_INTERNAL') || die();

// A description shown in the admin theme selector.
$string['choosereadme'] = 'Theme GOV.UK SWPDP is a child theme of the GOV.UK theme';
// The name of our plugin.
$string['pluginname'] = 'GOV.UK SWPDP';
// We need to include a lang string for each block region.
$string['region-side-pre'] = 'Right';

// nav hook strings
$string['myportfolio'] = 'My portfolio';
$string['learningmaterials'] = 'Learning materials';
// custom nav setting
$string['configtitle'] = 'GOVUK SWPDP theme settings';
$string['enablelearnercustomnav'] = 'Enable custom navigation items for learners';
$string['enablelearnercustomnav_desc'] = 'When enabled, learners see the custom navigation menu instead of the default.';