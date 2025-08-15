YUI.add('moodle-availability_save-form', function (Y, NAME) {

YUI.add('moodle-availability_save-form', function(Y) {
  M.availability_save = M.availability_save || {};
  M.availability_save.form = Y.Object(M.core_availability.plugin);

  M.availability_save.form.initInner = function(params) {
    this.databases = params.databases || [];
    this.targetcmid = params.targetcmid || 0;
  };

  M.availability_save.form.getNode = function(json) {
    json = json || {};

    var sel = '<select class="save-cmid"><option value="">--</option>';
    (this.databases || []).forEach(function(db) {
      sel += '<option value="'+db.cmid+'">'+Y.Escape.html(db.name)+'</option>';
    });
    sel += '</select>';

    var html =
      '<div class="availability-save">' +
        '<label>'+M.util.get_string("label_selectactivity","availability_save")+'</label> ' + sel +
      '</div>';

    var node = Y.Node.create('<span class="form-inline">'+html+'</span>');

    // Pre-fill.
    if (json.cmid) {
      node.one('.save-cmid').set('value', json.cmid);
    }

    // Update on change.
    node.one('.save-cmid').on('change', function() {
      M.core_availability.form.update();
    });

    return node;
  };

  M.availability_save.form.fillValue = function(value, node) {
    var cmid = parseInt(node.one('.save-cmid').get('value') || 0, 10);
    if (!cmid) { value = null; return; }

    if (this.targetcmid && cmid === this.targetcmid) {
      value = null; // forces an error via fillErrors
      return;
    }

    value.type = 'save';  // must match plugin directory name
    value.cmid = cmid;
  };

  M.availability_save.form.fillErrors = function(errors, node) {
    var cmid = parseInt(node.one('.save-cmid').get('value') || 0, 10);
    if (!cmid) {
      errors.push('availability_save:error_noactivity');
    }
    if (this.targetcmid && cmid && cmid === this.targetcmid) {
      errors.push('availability_save:error_sameactivity');
    }
  };
}, '@VERSION@', {requires:['base','node','event','escape','moodle-core_availability-form']});

}, '@VERSION@', {"requires": ["base", "node", "event", "escape", "moodle-core_availability-form"]});
