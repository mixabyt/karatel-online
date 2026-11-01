// Written by the launcher on every start; %GAME_URL% is the chosen server's game page.
user_pref("browser.startup.homepage", "%GAME_URL%");
user_pref("browser.startup.page", 1);
user_pref("browser.startup.homepage_override.mstone", "ignore");
user_pref("startup.homepage_welcome_url", "");
user_pref("startup.homepage_welcome_url.additional", "");
user_pref("browser.shell.checkDefaultBrowser", false);
user_pref("browser.rights.3.shown", true);
user_pref("browser.sessionstore.resume_from_crash", false);
user_pref("browser.sessionstore.max_resumed_crashes", 0);
user_pref("toolkit.startup.max_resumed_crashes", -1);
user_pref("browser.slowStartup.notificationDisabled", true);
user_pref("browser.disableResetPrompt", true);
user_pref("browser.tabs.warnOnClose", false);
user_pref("browser.tabs.drawInTitlebar", false);
user_pref("browser.link.open_newwindow", 2);
user_pref("browser.uitour.enabled", false);
user_pref("browser.newtabpage.enabled", false);
user_pref("extensions.pocket.enabled", false);

// The browser is frozen at 51: no updates, no blocklist, no telemetry prompts.
user_pref("app.update.enabled", false);
user_pref("app.update.auto", false);
user_pref("app.update.service.enabled", false);
user_pref("extensions.update.enabled", false);
user_pref("extensions.blocklist.enabled", false);
user_pref("browser.search.update", false);
user_pref("browser.safebrowsing.malware.enabled", false);
user_pref("browser.safebrowsing.phishing.enabled", false);
user_pref("browser.safebrowsing.downloads.enabled", false);
user_pref("datareporting.healthreport.uploadEnabled", false);
user_pref("datareporting.policy.dataSubmissionEnabled", false);
user_pref("datareporting.policy.dataSubmissionPolicyBypassNotification", true);
user_pref("toolkit.telemetry.enabled", false);
user_pref("toolkit.telemetry.reportingpolicy.firstRun", false);
user_pref("network.captive-portal-service.enabled", false);
user_pref("media.gmp-manager.updateEnabled", false);

// Unity Web Player: always active, no click-to-play, no outdated-plugin bars; plugins run in the
// classic single-process mode they were made for.
user_pref("plugin.default.state", 2);
user_pref("plugins.click_to_play", false);
user_pref("plugin.load_flash_only", false);
user_pref("plugins.hide_infobar_for_outdated_plugin", true);
user_pref("plugins.hide_infobar_for_missing_plugin", true);
user_pref("plugins.update.notifyUser", false);
user_pref("browser.tabs.remote.autostart", false);
user_pref("browser.tabs.remote.autostart.2", false);
