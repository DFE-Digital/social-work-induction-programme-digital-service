<?php
// theme/govuk_swpdp/classes/form/portfolio_form.php
namespace theme_govuk_swpdp\form;

defined('MOODLE_INTERNAL') || die();

use moodleform;
require_once($CFG->libdir . '/formslib.php');

class portfolio_form extends \moodleform {

    protected function definition() {
        global $DB;

        $mform = $this->_form;
        $courseid = $this->_customdata['courseid'];

        // Load all course sections
        $sections = $DB->get_records('course_sections', ['course' => $courseid], 'section ASC', 'id, section, name');
        $options = [];
        foreach ($sections as $s) {
            $label = $s->name ?: get_string('section') . ' ' . $s->section;
            $options[$s->id] = $label;
        }

        $mform->addElement('select', 'sectionid', get_string('chooseportfoliosection', 'theme_govuk_swpdp'), $options);
        $mform->setType('sectionid', PARAM_INT);

        $this->add_action_buttons(true, get_string('savechanges'));
    }
}
