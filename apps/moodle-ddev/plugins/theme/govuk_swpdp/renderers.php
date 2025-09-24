<?php

class theme_govuk_swpdp_core_renderer extends theme_govuk_core_renderer
{
    public function context_header($headerinfo = null, $headinglevel = 1): string {
        // Clear the heading on database activity page with no entries
        if ($this->page->pagetype === 'mod-data-view') {
            try {
                $manager = \mod_data\manager::create_from_coursemodule($this->page->cm);
                if (!$manager->has_records()) {
                    $this->page->set_heading('');
                }
            } catch (\Throwable $e) {}
        }

        return parent::context_header($headerinfo, $headinglevel);
    }
}
