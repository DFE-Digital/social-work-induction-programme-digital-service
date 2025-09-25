<?php

use theme_govuk_swpdp\helpers\common_helpers as Helpers;
use core\output\single_select;

class theme_govuk_swpdp_core_renderer extends theme_govuk_core_renderer
{
    /* Hide breadcrumbs on pages within a course */
    public function navbar(): string
    {
        if (
            $this->page->context &&
            $this->page->context->get_course_context(false)
        ) {
            return '';
        }
        return parent::navbar();
    }

    /* Empty header on view database page to hide course name 
    *  Add class for empty heading as css helper
    *  Add class when non staff as css helper to hide group info 
    *  Call ESM javascript to convert author link to span
    */
    public function header(): string {
        if ($this->page->pagetype === 'mod-data-view') {
            $this->page->set_heading('');
            $this->page->add_body_class('db-noheading');
            if (!$this->is_staff_viewing_database()) {
                $this->page->add_body_class('no-group-menu');
            }    
        }
        return parent::header();
    /* Remove the h2 on the database add/edit page */
    public function heading($text, $level = 2, $classes = 'main', $id = null) {
        if ($this->page->url->compare(new \moodle_url('/mod/data/edit.php'), URL_MATCH_BASE)) {
            $headingstomatch = [
                get_string('newentry', 'mod_data'),
                get_string('editentry', 'mod_data')
            ];
            $headingcontent = trim(strip_tags((string)$text));
            foreach ($headingstomatch as $target) {
                if ($headingcontent === trim($target)) {
                    return '';
                }
            }
        } 

        return parent::heading($text, $level, $classes, $id);
    }

    /* Hide groups dropdown on database view page for non staff */
    protected function render_single_select(single_select $select): string {
        if (!$this->is_staff_viewing_database()) {
            $data = $select->export_for_template($this);
            if ($data->name === 'group') {
                return '';
            }
        }
        $data = $select->export_for_template($this);
        return $this->render_from_template('core/single_select', $data);
    }

    private function is_staff_viewing_database(): bool {
        if ($this->page->pagetype !== 'mod-data-view') {
            return false;
        }
        global $USER;
        return Helpers::is_staff_like($USER->id);
    }

    /**
     * Override to change content used to render the sticky footer template
     * on the add entry page for database activities (mod/data/edit.php)
     *
     * @param \core\output\sticky_footer $footer
     * @return string
     */
    protected function render_sticky_footer(
        \core\output\sticky_footer $footer,
    ): string {
        $data = $footer->export_for_template($this);

        $isdataedit = $this->page->pagetype === 'mod-data-edit';

        if ($isdataedit) {
            $data['stickycontent'] = html_writer::empty_tag('input', [
                'type' => 'submit',
                'name' => 'saveandview',
                'value' => get_string('saveprogress', 'theme_govuk_swpdp'),
                'class' => 'govuk-button mx-1',
            ]);
            $data['stickycontent'] .= html_writer::link(
                '',
                get_string('sendtoobserver', 'theme_govuk_swpdp'),
                [
                    'class' => 'govuk-button govuk-button--secondary',
                    'role' => 'button',
                    'onclick' => 'return false;', // Makes the button have no functionality
                ],
            );
        }

        return $this->render_from_template('core/sticky_footer', $data);
    }

    public function firstview_fakeblocks(): bool {
        return false;
    }
}
