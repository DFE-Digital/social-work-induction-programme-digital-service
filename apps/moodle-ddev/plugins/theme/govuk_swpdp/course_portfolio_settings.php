<?php
// theme/govuk_swpdp/course_portfolio_settings.php
require('../../config.php');

$courseid = required_param('id', PARAM_INT);
$context = context_course::instance($courseid);
require_login($courseid);
require_capability('moodle/course:update', $context);

$url = new moodle_url('/theme/govuk_swpdp/course_portfolio_settings.php', ['id' => $courseid]);
$PAGE->set_url($url);
$PAGE->set_context($context);
$PAGE->set_heading(get_string('myportfolio', 'theme_govuk_swpdp'));
$PAGE->set_title(get_string('myportfolio', 'theme_govuk_swpdp'));

global $DB;

// Load the currently saved section if it exists
$currentsection = $DB->get_field('theme_govuk_swpdp_portfolio', 'sectionid', ['courseid' => $courseid]);

$mform = new theme_govuk_swpdp\form\portfolio_form($url, ['courseid' => $courseid]);

// Prepopulate form with the saved section
if ($currentsection) {
    $mform->set_data(['sectionid' => $currentsection]);
}

if ($mform->is_cancelled()) {
    redirect(new moodle_url('/course/view.php', ['id' => $courseid]));
} else if ($data = $mform->get_data()) {
    // Upsert the section ID for this course
    if ($record = $DB->get_record('theme_govuk_swpdp_portfolio', ['courseid' => $courseid])) {
        $record->sectionid = $data->sectionid;
        $DB->update_record('theme_govuk_swpdp_portfolio', $record);
    } else {
        $DB->insert_record('theme_govuk_swpdp_portfolio', [
            'courseid' => $courseid,
            'sectionid' => $data->sectionid,
        ]);
    }

    redirect(new moodle_url('/course/view.php', ['id' => $courseid]),
        get_string('settingssaved', 'theme_govuk_swpdp'));
}

echo $OUTPUT->header();
$mform->display();
echo $OUTPUT->footer();
