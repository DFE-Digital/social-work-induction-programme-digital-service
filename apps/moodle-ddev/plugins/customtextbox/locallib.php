<?php
defined('MOODLE_INTERNAL') || die();

class assign_submission_customtextbox extends \assign_submission_plugin {

    /** human‐readable name */
    public function get_name() {
        return get_string('pluginname', 'assignsubmission_customtextbox');
    }

    /** enabled for this assignment? */
    public function is_enabled() {
        return (bool)$this->get_config('enabled');
    }

    /**
     * Add “enable” box and description editor to the Assignment settings form.
     *
     * @param \MoodleQuickForm $mform the form being built
     * @return void
     */
    public function get_settings(\MoodleQuickForm $mform): void {
        // Add only the description editor, not the enable-box.
        $editoroptions = ['subdirs'=>0, 'maxfiles'=>0, 'maxbytes'=>0];
        $mform->addElement('editor',
            'assignsubmission_customtextbox_textdescription',
            get_string('textdescription','assignsubmission_customtextbox'),
            null, $editoroptions
        );
        $mform->setType('assignsubmission_customtextbox_textdescription', PARAM_RAW);

        // Hide it unless the core checkbox is ticked: REVISIT
        //   fieldToHide, fieldToCheck, operator, value
        $mform->hideIf(
        'assignsubmission_customtextbox_textdescription',
        'assignsubmission_customtextbox_enabled',
        'eq',
        '0'
        );
    }


    /** add settings to assignment config form */
    public function get_settings_form(\MoodleQuickForm $mform, $data) {
        // Nothing here; settings.php handled it.
    }

    /**
     * Store the assignment‐instance settings for this plugin.
     *
     * @param \stdClass $data The data submitted from the assignment edit form.
     * @return bool
     */
    public function save_settings(\stdClass $data): bool {
        $this->set_config(
            'enabled',
            !empty($data->assignsubmission_customtextbox_enabled)
        );
        
        $this->set_config(
            'textdescription',
            $data->assignsubmission_customtextbox_textdescription['text']
        );
        return true;
    }

    /**
     * Add textbox (and description) to the submission form.
     *
     * @param \stdClass|null      $submission the existing submission record (if any)
     * @param \MoodleQuickForm    $mform      the form to add elements to
     * @param \stdClass           $data       default values for the form
     * @return bool               true if we added elements
     */
    public function get_form_elements($submission, \MoodleQuickForm $mform, \stdClass $data): bool {
        // show description if set
        $desc = (string) $this->get_config('textdescription');
        if ($desc !== '') {
            $formatted = format_text(
                $desc,
                FORMAT_HTML,
                [
                    'noclean' => true,
                    // Give it the right context for filters, file URLs, etc.
                    'context' => $this->assignment->get_context(),
                ]
            );
            $mform->addElement('static', 'customtextbox_desc', '', $formatted);
        }

         // the custom field text area
        $mform->addElement('textarea', 'customtextbox_textcontent',
            get_string('textcontent', 'assignsubmission_customtextbox'),
            'wrap="virtual" rows="10" cols="80"');
        $mform->setType('customtextbox_textcontent', PARAM_TEXT);

        // **Re-apply the saved value if there is one** - REVISIT
        if (!empty($data->customtextbox_textcontent)) {
            $mform->setDefault(
                'customtextbox_textcontent',
                $data->customtextbox_textcontent
            );
        }

        return true; 
    }

    public function data_preprocessing(&$defaultvalues) {
        global $DB;
        if (!empty($defaultvalues->id) &&
            $record = $DB->get_record('assignsubmission_customtextbox', ['submission' => $defaultvalues->id])) {
            $defaultvalues->customtextbox_textcontent = $record->textcontent;
            debugging ('This is the retrieved text:' . $DB->get_record('assignsubmission_customtextbox', ['submission' => $defaultvalues->id]),  DEBUG_DEVELOPER);
        }
    } 

    /**
     * Save any custom submission data.
     *
     * @param \stdClass $submission The submission record or grade object.
     * @param \stdClass $data       The raw form data.
     * @return bool
     */
    public function save(\stdClass $submission, \stdClass $data): bool {
        global $DB;
    
        // Pull textbox content out of the form data.
        $content = isset($data->customtextbox_textcontent)
            ? $data->customtextbox_textcontent
            : '';
    
        // Prepare the record object.
        $record = new \stdClass();
        $record->assignment  = $this->assignment->get_instance()->id;
        $record->submission  = $submission->id;
        $record->textcontent = $content;
    
        // Check if we already have one for this submission.
        $existing = $DB->get_record(
            'assignsubmission_customtextbox',
            ['submission' => $submission->id],
            'id'
        );
    
        if ($existing) {
            // Update the existing row.
            $record->id = $existing->id;
            $DB->update_record('assignsubmission_customtextbox', $record);
        } else {
            // Insert a new one.
            $DB->insert_record('assignsubmission_customtextbox', $record);
        }
    
        return true; 
    }
    
    /**
     * Display the saved text in the grading interface.
     *
     * @param \stdClass $submission The submission record.
     * @return string HTML to display.
     */
    public function view(\stdClass $submission) {
        global $DB;

        $record = $DB->get_record(
            'assignsubmission_customtextbox',
            ['submission' => $submission->id]
        );

        if ($record && (string)$record->textcontent !== '') {
            $renderer = $this->assignment->get_renderer();
            return $renderer->box(
                format_text($record->textcontent, FORMAT_PLAIN),
                'assignsubmission_customtextbox'
            );
        }
        return '';
    }

}