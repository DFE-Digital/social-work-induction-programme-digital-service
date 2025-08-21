<?php
defined('MOODLE_INTERNAL') || die();

class data_field_pqsoutcomes extends data_field_base {
    /** @var string */
    public $type = 'pqsoutcomes';

    public function display_name() {
        return format_string($this->field->name ?: get_string('pluginname', 'datafield_pqsoutcomes'));
    }

    /** Render the add/edit UI for a Database entry (student view). */
    public function display_add_field($recordid = 0, $formdata = null) {
        global $DB;

        $courseid = (int)$this->data->course;

        // Fetch the course's linked competencies (public API).
        $list = \core_competency\course_competency::list_course_competencies($courseid);

        $json = json_encode($list, JSON_PRETTY_PRINT | JSON_INVALID_UTF8_SUBSTITUTE);
        echo "\n<!-- pqsoutcomes debug: list_course_competencies\n{$json}\n-->\n";

        // Load previously-selected IDs for this entry (CSV like ",12,34,").
        $selected = [];
        if ($recordid) {
            if ($content = $DB->get_record('data_content',
                    ['fieldid' => $this->field->id, 'recordid' => $recordid])) {
                $selected = array_filter(array_map('intval',
                    explode(',', trim((string)$content->content, ','))));
            }
        }

        // Normalise the API return into [competencyid => label].
        $options = [];
        foreach ((array)$list as $cc) {
            // Accept either a persistent object, stdClass, or array.
            $cid = 0;
            if (is_object($cc)) {
                if (method_exists($cc, 'get')) {
                    $cid = (int)$cc->get('competencyid');
                } elseif (isset($cc->competencyid)) {
                    $cid = (int)$cc->competencyid;
                } elseif (isset($cc->id)) {
                    // Some calls return the competency itself.
                    $cid = (int)$cc->id;
                }
            } elseif (is_array($cc)) {
                $cid = (int)($cc['competencyid'] ?? $cc['id'] ?? 0);
            }
            if (!$cid) { continue; }

            $comp = \core_competency\competency::get_record(['id' => $cid]);
            if (!$comp) { continue; }
            $label = $comp->get('shortname') ?: $comp->get('idnumber') ?: ('ID '.$cid);
            $options[$cid] = $label;
        }

        $name = 'field_' . $this->field->id . '[]';
        $html = html_writer::start_div('datafield-pqsoutcomes');
        $html .= html_writer::tag('div', get_string('pqshelp','datafield_pqsoutcomes'), ['class'=>'dimmed_text']);

        if (empty($options)) {
            $html .= html_writer::div(get_string('nocomps','datafield_pqsoutcomes'));
        } else {
            $html .= html_writer::start_tag('ul', ['class'=>'list-unstyled']);
            foreach ($options as $cid => $label) {
                $idattr = 'pqsoutcomes_'.$this->field->id.'_'.$cid;
                $attrs = ['type'=>'checkbox','name'=>$name,'id'=>$idattr,'value'=>$cid];
                if (in_array((int)$cid, $selected, true)) { $attrs['checked'] = 'checked'; }

                $html .= html_writer::tag('li',
                    html_writer::empty_tag('input', $attrs).' '.
                    html_writer::label(s($label), $idattr)
                );
            }
            $html .= html_writer::end_tag('ul');
        }

        $html .= html_writer::end_div();
        return $html;
    }

    /** Save (store CSV of competency IDs in data_content->content). */
    public function update_content($recordid, $value, $name = '') {
        global $DB;

        $param = 'field_' . $this->field->id;
        $vals = is_array($value) ? $value : optional_param_array($param, [], PARAM_INT);
        $ids = array_values(array_unique(array_map('intval', $vals)));
        sort($ids);
        $csv = $ids ? ',' . implode(',', $ids) . ',' : '';

        if ($content = $DB->get_record('data_content',
                ['fieldid'=>$this->field->id,'recordid'=>$recordid])) {
            $content->content = $csv;
            $DB->update_record('data_content', $content);
        } else {
            $DB->insert_record('data_content', (object)[
                'fieldid'=>$this->field->id, 'recordid'=>$recordid, 'content'=>$csv
            ]);
        }
        return true;
    }

    /** Display on browse/single templates. */
    public function display_browse_field($recordid, $template) {
        global $DB;
        $labels = [];
        if ($content = $DB->get_record('data_content', ['fieldid'=>$this->field->id,'recordid'=>$recordid])) {
            $ids = array_filter(array_map('intval', explode(',', trim((string)$content->content, ','))));
            if ($ids) {
                list($insql, $params) = $DB->get_in_or_equal($ids, SQL_PARAMS_NAMED);
                $records = $DB->get_records_select('competency', "id $insql", $params, '', 'id,shortname,idnumber');
                foreach ($ids as $id) {
                    if (isset($records[$id])) {
                        $r = $records[$id];
                        $labels[] = s($r->shortname ?: $r->idnumber ?: ('ID '.$id));
                    }
                }
            }
        }
        return $labels ? html_writer::alist($labels, ['class'=>'list-unstyled']) : '';
    }

    /** Optional: render search filter for this field (DB activity page). */
    public function display_search_field($search = '', $name = '', $label = '') {
        // You can just return empty string if search is not needed.
        return '';
    }
}