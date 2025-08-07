/* eslint-disable no-console */     // allow console.log for now for grunt amd eslinting
import $ from 'jquery';
import { addNotification } from 'core/notification';

/**
 * @module local_manualsavegroup/manualsavegroup
 */

/**
 * Initialise manual save group behaviour on the members.php page.
 *
 * Hide the default add/remove buttons, inject “Save changes” and move controls,
 * and handle batching of add/remove via AJAX.
 *
 * @returns {void}
 */
export const init = () => {
    console.log('manualsavegroup.init running');

    // hide the built-in arrows
    $('#add, #remove').hide();

    // inject “Save changes” button
    const form = $('#assignform');
    if (!form.length) {
        return;
    }

    if (!$('#batch-save-btn').length) {
        form.append(
            '<button id="batch-save-btn" type="submit" ' +
            'class="btn btn-primary mb-2">Save changes</button>'
        );
    }

    // inject Move buttons between the selects
    if (!$('#btn-move-add').length) {
        const moveControls = $(
            '<div id="move-controls" style="margin: .5em 0; text-align:center;">' +
            '<button type="button" id="btn-move-add"    class="btn btn-secondary btn-sm">Add Selected  ←</button>&nbsp;' +
            '<button type="button" id="btn-move-remove" class="btn btn-secondary btn-sm">→  Remove Selected</button>' +
            '</div>'
        );
        $('#buttonscell').html(moveControls);
    }

    // add member handler
    $('#btn-move-add').on('click', () => {
        $('#addselect option:selected')
            .prop('selected', false)
            .appendTo('#removeselect');
    });

    // remove member handler
    $('#btn-move-remove').on('click', () => {
        $('#removeselect option:selected')
            .prop('selected', false)
            .appendTo('#addselect');
    });

    // take a snapshot of the original group members
    const original = $('#removeselect option')
        .map((i, o) => o.value)
        .get();

    form.off('submit.manualsave').on('submit.manualsave', async function (e) {
        // If the user clicks the "back to groups" button, action it
        const trigger = e.originalEvent && e.originalEvent.submitter;
        if (trigger && trigger.name === 'cancel') {
            return;
        }
        e.preventDefault();

        // read current list
        const current = $('#removeselect option')
            .map((i, o) => o.value)
            .get();

        // compute who was added/removed
        const toAdd = current.filter(id => !original.includes(id));
        const toRemove = original.filter(id => !current.includes(id));

        console.log('🛠️ toAdd=', toAdd, 'toRemove=', toRemove);

        const sesskey = form.find('input[name="sesskey"]').val();
        const actionUrl = form.attr('action');

        /**
         * Submit a batch of add/remove requests to the group members page.
         *
         * @param {'add'|'remove'} buttonName - Name of the submit button ("add" or "remove").
         * @param {string} fieldName - Name of the select field ("addselect" or "removeselect").
         * @param {string[]} ids - Array of user ID strings.
         * @returns {Promise<void>}
         */
        async function postChange(buttonName, fieldName, ids) {
            if (!ids.length) {
                return;
            }
            const data = new URLSearchParams();
            data.append('sesskey', sesskey);
            data.append(buttonName, '1');
            ids.forEach(id => data.append(`${fieldName}[]`, id));
            await fetch(actionUrl, {
                method: 'POST',
                credentials: 'same-origin',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: data.toString()
            });
        }

        // add members
        await postChange('add', 'addselect', toAdd);
        // remove members
        await postChange('remove', 'removeselect', toRemove);

        // notification below not working - would need to explore further
        // notify & reload
        addNotification({
            message: M.util.get_string('changessaved', 'local_manualsavegroup'),
            type: 'success',
            closebutton: true,
            announce: true
        });
        window.location.reload();

    });
};
