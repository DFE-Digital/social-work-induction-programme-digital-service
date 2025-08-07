<?php
namespace local_selectgroupmembers\form;

defined('MOODLE_INTERNAL') || die();

global $CFG;
require_once($CFG->libdir . '/formslib.php');
require_once($CFG->libdir . '/accesslib.php');  // for role and permission APIs
require_once($CFG->dirroot . '/group/lib.php');   // for groups_get_members()

class membersform extends \moodleform {
    /**
     * Define the form.
     *
     * @throws \moodle_exception
     */
    public function definition() {
        $mform    = $this->_form;
        $courseid = $this->_customdata['courseid'];
        $groupid  = $this->_customdata['groupid'];

        // Course context and roles
        $context = \context_course::instance($courseid);
        $roles   = \get_roles_used_in_context($context);

        // Fetch current group members for pre-selection
        $currentmembers = array_map(
            function($u) { return $u->id; },
            groups_get_members($groupid)
        );

        $allusers = [];
        
        global $OUTPUT;
        foreach ($roles as $role) {
            // Users assigned this role in this context
            $users = \get_role_users(
                $role->id,
                $context,
                false,
                'u.*',
                'u.lastname'
            );
            if (empty($users)) {
                continue;
            }

            $rolename = role_get_name($role, $context);
            $mform->addElement('html',
                $OUTPUT->heading(format_string($rolename), 3)
            );

            // Build a list of checkboxes for users in this role
            $checkboxes = [];
            foreach ($users as $u) {
                $label = fullname($u) . ' (' . s($u->email) . ')';
                $name  = 'memberids[' . $u->id . ']';
                $el    = $mform->createElement('checkbox', $name, '', $label);
                if (in_array($u->id, $currentmembers, true)) {
                    $mform->setDefault($name, 1);
                }
                $checkboxes[] = $el;

                $allusers[$u->id] = $label;
            }

            $mform->addGroup(
                $checkboxes,
                'group_' . $role->id . '_members',
                '',
                ['<br />'],
                false
            );
        }

        
        $currentmembers = array_map(
            function($u){ return $u->id; },
            groups_get_members($this->_customdata['groupid'])
        );
        $mform->addElement('hidden',
            'originalmembers',
            implode(',', $currentmembers)
        );
        $mform->setType('originalmembers', PARAM_RAW);
        
        $mform->addElement('hidden', 'group', $this->_customdata['groupid']);
        $mform->setType('group', PARAM_INT);

        // Action buttons: Save and Cancel
        $this->add_action_buttons(true, get_string('savechanges', 'local_selectgroupmembers'));
    }
}
