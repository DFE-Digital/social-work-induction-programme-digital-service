<?php
defined('MOODLE_INTERNAL') || die();

if ($hassiteconfig) {
    $settings = new admin_settingpage(
        'themesettinggovuk_swpdp',
        new lang_string('configtitle', 'theme_govuk_swpdp')
    );

    if ($ADMIN->fulltree) {
        $setting = new admin_setting_configcheckbox(
            'theme_govuk_swpdp/enablelearnercustomnav',
            new lang_string('enablelearnercustomnav', 'theme_govuk_swpdp'),
            new lang_string('enablelearnercustomnav_desc', 'theme_govuk_swpdp'),
            1 // on by default 
        );
        $setting->set_updatedcallback('theme_reset_all_caches');
        $settings->add($setting);
    }
}