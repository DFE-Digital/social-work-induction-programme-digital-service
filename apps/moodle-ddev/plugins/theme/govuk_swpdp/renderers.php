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

    /* Empty header on view database page to hide course name */
    public function header(): string {
        if ($this->page->pagetype === 'mod-data-view') {
            $this->page->set_heading('');
            $this->page->add_body_class('db-noheading');
        }
        return parent::header();
    }    

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
}
