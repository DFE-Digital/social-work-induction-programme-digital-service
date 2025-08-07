<?php

require_once(__DIR__ . '/../../config.php');

global $DB, $PAGE, $OUTPUT;

$groupid = required_param('group', PARAM_INT);
$group   = $DB->get_record('groups', ['id' => $groupid], '*', MUST_EXIST);
$course  = get_course($group->courseid);

require_login($course);

// Set the page context for capability checks and navigation
$context = context_course::instance($course->id);
$PAGE->set_context($context);

require_capability('moodle/course:managegroups', $context);

$url = new \moodle_url('/local/selectgroupmembers/index.php', ['group' => $groupid]);
$PAGE->set_url($url);
$PAGE->set_pagelayout('admin');
$PAGE->set_title(get_string('pluginname', 'local_selectgroupmembers'));
$PAGE->set_heading(format_string($course->fullname));

$mform = new \local_selectgroupmembers\form\membersform(
    null,
    ['courseid' => $course->id, 'groupid' => $groupid]
);

if ($mform->is_cancelled()) {
    redirect(new \moodle_url('/course/view.php', ['id' => $course->id]));
}

if ($data = $mform->get_data()) {
    $original = array_map('intval', explode(',', $data->originalmembers));
    $submitted = array_keys($data->memberids ?? []);
    $submitted = array_map('intval', $submitted);

    $toadd    = array_diff($submitted, $original);
    $toremove = array_diff($original, $submitted);

    // Apply group membership changes
    foreach ($toadd as $uid) {
        groups_add_member($groupid, $uid);
    }
    foreach ($toremove as $uid) {
        groups_remove_member($groupid, $uid);
    }

    \core\notification::success(
        get_string('changessaved', 'local_selectgroupmembers')
    );

    redirect($url);
}

// Output the page
echo $OUTPUT->header();
echo $OUTPUT->heading(get_string('pluginname', 'local_selectgroupmembers'));

// Display the form
$mform->display();

echo $OUTPUT->footer();
