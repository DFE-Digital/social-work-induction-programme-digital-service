<?php
namespace availability_save;

defined('MOODLE_INTERNAL') || die();

use core_availability\info;
use coding_exception;

/**
 * Availability condition: require that the user has at least one entry
 * in a specified (watched) Database activity (mod_data).
 */
class condition extends \core_availability\condition {
    /** @var int course module id of the prerequisite Database activity (watched) */
    protected $cmid;

    /** cache final results for this request: ["watchedcmid:userid:targetcmid" => bool] */
    protected static $resultcache = [];

    /** per-request cache for "has entry in data cm": ["cmid:userid" => bool] */
    protected static $entrycache = [];

    public function __construct($structure) {
        if (!isset($structure->cmid) || !is_numeric($structure->cmid)) {
            throw new coding_exception('availability_save: missing cmid');
        }
        $this->cmid = (int)$structure->cmid;
    }

    public function save() {
        return (object)[
            'type' => 'save',   
            'cmid' => $this->cmid,
        ];
    }

    public function is_available($not, info $info, $grabthelot, $userid) {
        $allow = $this->evaluate($userid, $info);
        if ($not) {
            $allow = !$allow;
        }
        return $allow;
    }

    public function get_description($full, $not, info $info) {
        $modinfo = get_fast_modinfo($info->get_course());
        $name = isset($modinfo->cms[$this->cmid]) ? $modinfo->cms[$this->cmid]->name : get_string('activity');
        return $not
            ? get_string('display_notyet', 'availability_save', $name)
            : get_string('display_requires', 'availability_save', $name);
    }

    protected function get_debug_string() {
        return 'cmid=' . $this->cmid . ' (watched)';
    }

    /**
     * Logic:
     * - If user has an entry in the watched Database -> available.
     * - ELSE, if the TARGET module is a Database and user has an entry there -> available.
     * - ELSE -> not available.
     */
    protected function evaluate(int $userid, info $info): bool {
        if ($userid <= 0) {
            return false;
        }

        // Identify the target cmid (the module this condition is attached to).
        $ctx = $info->get_context();
        $targetcmid = (int)$ctx->instanceid; // availability on modules uses context_module.

        $finalkey = $this->cmid . ':' . $userid . ':' . $targetcmid;
        if (array_key_exists($finalkey, self::$resultcache)) {
            return self::$resultcache[$finalkey];
        }

        if ($targetcmid === $this->cmid) {
            return self::$resultcache[$finalkey] = false;
        }

        $modinfo = get_fast_modinfo($info->get_course());

        // 1) Primary check: entry exists in the watched Database.
        $watchedok = $this->user_has_entry_in_dbcm($userid, $this->cmid, $modinfo);
        if ($watchedok) {
            return self::$resultcache[$finalkey] = true;
        }

        // 2) Fallback: if the TARGET is a Database and the user has an entry there, keep it available.
        $targetok = $this->user_has_entry_in_dbcm($userid, $targetcmid, $modinfo);

        return self::$resultcache[$finalkey] = $targetok;
    }

    /**
     * Returns true if $userid has at least one entry in the Database represented by $cmid.
     * If $cmid is not a Database cm (or invalid), returns false.
     *
     * @param int $userid
     * @param int $cmid Database coursemodule id to check
     * @param \course_modinfo $modinfo
     * @return bool
     */
    protected function user_has_entry_in_dbcm(int $userid, int $cmid, \course_modinfo $modinfo): bool {
        global $DB;

        if ($userid <= 0 || $cmid <= 0) {
            return false;
        }

        $cachekey = $cmid . ':' . $userid;
        if (array_key_exists($cachekey, self::$entrycache)) {
            return self::$entrycache[$cachekey];
        }

        // Validate cm and ensure it's a Database activity.
        if (empty($modinfo->cms[$cmid])) {
            return self::$entrycache[$cachekey] = false;
        }
        $cm = $modinfo->cms[$cmid];
        if ($cm->modname !== 'data' || $cm->deletioninprogress) {
            return self::$entrycache[$cachekey] = false;
        }

        // Database instance id (data.id)
        $dataid = (int)$cm->instance;

        // Fast EXISTS on data_records by user.
        $sql = "SELECT 1
                  FROM {data_records}
                 WHERE dataid = :dataid
                   AND userid = :userid";
        $found = $DB->record_exists_sql($sql, ['dataid' => $dataid, 'userid' => $userid]);

        return self::$entrycache[$cachekey] = $found;
    }

    // Handle backup/restore id shifts if the watched cmid changes.
    public function update_dependency_id($table, $oldid, $newid) {
        if ($table === 'course_modules' && (int)$this->cmid === (int)$oldid) {
            $this->cmid = (int)$newid;
            return true;
        }
        return false;
    }
}