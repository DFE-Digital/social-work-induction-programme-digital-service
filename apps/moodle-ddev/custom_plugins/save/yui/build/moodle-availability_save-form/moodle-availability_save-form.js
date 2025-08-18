YUI.add('moodle-availability_save-form', function (Y, NAME) {

/**
 * @module moodle-availability_save-form
 */
M.availability_save = M.availability_save || {};
M.availability_save.form = Y.Object(M.core_availability.plugin);

M.availability_save.form.initInner = function(params) {
  params = params || {};
  this.databases  = params.databases || [];
  this.targetcmid = params.targetcmid || 0;
};

M.availability_save.form.getNode = function(json) {
  json = json || {};

  var label = (M.util && M.util.get_string)
    ? M.util.get_string('label_selectactivity','availability_save')
    : 'Entry saved for database';

  var options = ['<option value="">--</option>'];
  if (Array.isArray(this.databases) && this.databases.length) {
    this.databases.forEach(function(db) {
      if (!db || typeof db.cmid === 'undefined') { return; }
      var name = (typeof db.name === 'string' && db.name.length) ? db.name : ('Database ' + db.cmid);
      options.push('<option value="' + db.cmid + '">' + Y.Escape.html(name) + '</option>');
    });
  }

  var html =
    '<span class="form-inline availability-save">' +
      '<label>' + label + '</label> ' +
      '<select class="save-cmid">' + options.join('') + '</select>' +
    '</span>';

  var node = Y.Node.create(html);

  var sel = node.one('.save-cmid');
  if (json.cmid && sel) { sel.set('value', String(json.cmid)); }
  if (sel) { sel.on('change', function() { M.core_availability.form.update(); }); }

  return node;
};

M.availability_save.form.fillValue = function(value, node) {
  var sel = node && node.one ? node.one('.save-cmid') : null;
  var cmid = sel ? parseInt(sel.get('value') || 0, 10) : 0;
  if (!cmid) { value = null; return; }
  if (this.targetcmid && cmid === this.targetcmid) { value = null; return; }
  value.type = 'save';
  value.cmid = cmid;
};

M.availability_save.form.fillErrors = function(errors, node) {
  var sel = node && node.one ? node.one('.save-cmid') : null;
  var cmid = sel ? parseInt(sel.get('value') || 0, 10) : 0;
  if (!cmid) { errors.push('availability_save:error_noactivity'); }   
  if (this.targetcmid && cmid && cmid === this.targetcmid) {
    errors.push('availability_save:error_sameactivity');              
  }
};


}, '@VERSION@', {"requires": ["base", "node", "event", "escape", "moodle-core_availability-form"]});
