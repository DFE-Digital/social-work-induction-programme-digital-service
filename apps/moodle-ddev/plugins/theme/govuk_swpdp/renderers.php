<?php

class theme_govuk_swpdp_core_renderer extends theme_govuk_core_renderer
{
    public function header(): string {
        if ($this->page->pagetype === 'mod-data-view') {
            $this->page->set_heading('');
        }
        return parent::header();
    }    

    /** Hide breadcrumbs on pages within a course */
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
}
