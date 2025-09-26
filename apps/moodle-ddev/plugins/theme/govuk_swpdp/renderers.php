<?php

class theme_govuk_swpdp_core_renderer extends core_renderer
{
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
        $rid = optional_param('rid', 0, PARAM_INT);
        $isadd = $rid == 0;

        if ($isdataedit && $isadd) {
            // include cancel and save buttons
            $backto = optional_param('backto', '', PARAM_LOCALURL);
            if (!empty($backto)) {
                $cancelurl = new \moodle_url($backto);
            } else {
                $cmid = $this->page->cm
                    ? $this->page->cm->id
                    : optional_param('id', 0, PARAM_INT);
                $cancelurl = new \moodle_url('/mod/data/view.php', [
                    'id' => $cmid,
                ]);
            }

            $data['stickycontent'] = html_writer::empty_tag('input', [
                'type' => 'submit',
                'name' => 'saveandview',
                'value' => get_string('saveprogress', 'theme_govuk_swpdp'),
                'class' => 'govuk-button mx-1',
            ]);
            $data['stickycontent'] .= html_writer::link(
                '',
                get_string('sendtoobserver', 'theme_govuk_swpdp'),
                ['class' => 'govuk-button govuk-button--secondary', 'role' => 'button'],
            );
        }

        return $this->render_from_template('core/sticky_footer', $data);
    }

    public function firstview_fakeblocks(): string {
        return '';
    }
}