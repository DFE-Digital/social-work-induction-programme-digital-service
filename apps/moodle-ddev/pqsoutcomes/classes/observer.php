<?php
namespace datafield_pqsoutcomes;

defined('MOODLE_INTERNAL') || die();

class observer {

    /**
     * Triggered when a course module completion is updated (incl. "Mark as done").
     */
    public static function on_cm_completion(\core\event\course_module_completion_updated $event): bool {
        global $DB;

        // Optional site-wide enable switch:
        // if (!get_config('datafield_pqsoutcomes', 'enabled')) { return true; }

        // Only act when the activity becomes complete (manual/auto).
        $state = (int)($event->other['completionstate'] ?? 0);
        if (!in_array($state, [COMPLETION_COMPLETE, COMPLETION_COMPLETE_PASS, COMPLETION_COMPLETE_FAIL], true)) {
            return true;
        }

        $cmid   = (int)$event->contextinstanceid;
        $userid = (int)$event->relateduserid ?: 0;
        if (!$userid) { return true; }

        error_log("111111111111 |||||||||||||||||||||||||||||||||||||||||^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^||||||||||||||||||||||||||||||||||||||||||||");

        // Make sure it's a Database activity.
        $cm = get_coursemodule_from_id(null, $cmid, 0, false, MUST_EXIST);
        if ($cm->modname !== 'data') {
            return true;
        }

        error_log("22222222222 |||||||||||||||||||||||||||||||||||||||||^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^||||||||||||||||||||||||||||||||||||||||||||");

        // Competencies feature must be enabled.
        if (!\core_competency\api::is_enabled()) {
            error_log("pqsoutcomes: competencies disabled site-wide");
            return true;
        }

        // Find the PQS field instance for this Database.
        $field = $DB->get_record('data_fields', ['dataid' => $cm->instance, 'type' => 'pqsoutcomes']);
        if (!$field) { return true; }

        // Teacher/admin-chosen outcome (saved on the field). Default to 1=Attach evidence.
        $ruleoutcome = (int)($field->param1 ?? 1);
        if (!in_array($ruleoutcome, [0,1,2,3], true)) {
            $ruleoutcome = 1;
        }
        if ($ruleoutcome === 0) { // Explicit "Do nothing".
            return true;
        }

        error_log("3333333333 |||||||||||||||||||||||||||||||||||||||||^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^||||||||||||||||||||||||||||||||||||||||||||");

        // Collect competencies selected by this user in this Database.
        $sql = "SELECT dc.content
                  FROM {data_content} dc
                  JOIN {data_records} dr ON dr.id = dc.recordid
                 WHERE dr.dataid = :dataid
                   AND dr.userid = :userid
                   AND dc.fieldid = :fieldid";
        $rows = $DB->get_records_sql($sql, [
            'dataid'  => $cm->instance,
            'userid'  => $userid,
            'fieldid' => $field->id
        ]);

        $selected = [];
        foreach ($rows as $r) {
            $ids = array_filter(array_map('intval', explode(',', trim((string)$r->content, ','))));
            $selected = array_merge($selected, $ids);
        }
        $selected = array_values(array_unique($selected));

        if (empty($selected)) {
            error_log("pqsoutcomes: No competencies found for user {$userid}, cmid {$cmid}");
            return true;
        }

        error_log("44444444444 |||||||||||||||||||||||||||||||||||||||||^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^||||||||||||||||||||||||||||||||||||||||||||");

        foreach ($selected as $competencyid) {
            error_log("pqsoutcomes: Applying outcome {$ruleoutcome} for competency {$competencyid}, user {$userid}");
            self::apply_outcome($cm, $userid, (int)$competencyid, $ruleoutcome);
        }

        error_log("5555555555 |||||||||||||||||||||||||||||||||||||||||^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^||||||||||||||||||||||||||||||||||||||||||||");

        return true;
    }

    /**
     * Apply the configured outcome to one competency for the given user.
     */
    protected static function apply_outcome(\stdClass $cm, int $userid, int $competencyid, int $ruleoutcome): void {
        $courseid  = (int)$cm->course;
        $context   = \context_module::instance($cm->id);
        $coursectx = \context_course::instance($courseid);

        // URL pointing back to the learner's entries in this Database activity.
        $entryurl = new \moodle_url('/mod/data/view.php', ['id' => $cm->id, 'mode' => 'list', 'u' => $userid]);

        switch ($ruleoutcome) {
            case 1: // Attach evidence (COURSE competency evidence)
            case 2: // Attach evidence + request review
            {
                // Ensure the per-course user competency exists (creates if missing).
                $uc = self::get_or_create_ucic($courseid, $userid, $competencyid);
                if (!$uc) { error_log('pqsoutcomes: could not get/create user_competency_course'); break; }

                $action        = \core_competency\evidence::ACTION_LOG;
                // Use the description key that accepts {$a} and pass the entry URL in $desca:
                $descidentifier = 'evidencedesc';
                $desccomponent  = 'datafield_pqsoutcomes';
                $desca          = $entryurl->out(false);   // populates {$a} in 'evidencedesc'
                $recommend      = ($ruleoutcome === 2);    // <-- this is the "Send for review"
                $url            = $entryurl->out(false);

                \core_competency\api::add_evidence(
                    $userid,
                    $competencyid,
                    $context->id,
                    $action,
                    $descidentifier,
                    $desccomponent,
                    $desca,
                    $recommend,
                    $url
                );

                break;
            }

            case 3: // Complete competency (grade) or fall back to review
            {
                if (has_capability('moodle/competency:competencygrade', $coursectx)) {
                    $gradevalue = self::default_proficient_value($competencyid);
                    if ($gradevalue) {
                        \core_competency\api::grade_competency_in_course(
                            $courseid,
                            $userid,
                            $competencyid,
                            $gradevalue,
                            get_string('automatedvia', 'datafield_pqsoutcomes')
                        );
                    } else {
                        \core_competency\api::request_review_of_user_competency_in_course($courseid, $userid, $competencyid);
                    }
                } else {
                    \core_competency\api::request_review_of_user_competency_in_course($courseid, $userid, $competencyid);
                }
                break;
            }

            default:
                // 0 or unknown => do nothing.
                break;
        }
    }

    /** Get (or create) the user_competency_in_course persistent. */
    protected static function get_or_create_ucic(
        int $courseid,
        int $userid,
        int $competencyid
    ): ?\core_competency\user_competency_course {

        // Try to fetch via API first.
        $uc = \core_competency\api::get_user_competency_in_course($courseid, $userid, $competencyid);
        if ($uc instanceof \core_competency\user_competency_course) {
            return $uc;
        }

        // Create a minimal record if it doesn't exist yet.
        $rec = (object)[
            'userid'       => $userid,
            'courseid'     => $courseid,
            'competencyid' => $competencyid,
        ];
        $uc = new \core_competency\user_competency_course(0, $rec);
        $uc->create();

        // (Optional) debug to prove type:
        error_log('pqsoutcomes: created UCiC of class ' . get_class($uc));

        return $uc;
    }

    /**
     * Get the framework’s default "proficient" scale value.
     */
    protected static function default_proficient_value(int $competencyid): int {
        $c = \core_competency\competency::get_record(['id' => $competencyid]);
        if (!$c) { return 0; }
        $framework = $c->get_framework();
        if (!$framework) { return 0; }
        $cfg = json_decode($framework->get('scaleconfiguration'), true) ?: [];
        if (!empty($cfg['default'])) {
            return (int)$cfg['default'];
        }
        if (!empty($cfg['proficient']) && is_array($cfg['proficient'])) {
            return (int)reset($cfg['proficient']);
        }
        return 0;
    }
}