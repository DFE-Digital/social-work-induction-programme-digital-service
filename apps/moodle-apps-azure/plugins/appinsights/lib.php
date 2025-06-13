<?php
// This file is loaded by Moodle on every page request if the plugin is installed.
defined('MOODLE_INTERNAL') || die();

use MoodleAppInsights\ApplicationInsights\Channel\Telemetry_Context;
use MoodleAppInsights\ApplicationInsights\Channel\Telemetry_Channel;
use MoodleAppInsights\ApplicationInsights\Telemetry_Client;
use MoodleAppInsights\ApplicationInsights\Telemetry_Context_Data_Severity_Level;

// Global instance of the telemetry client to use across Moodle
$GLOBALS['APPINSIGHTS_TELEMETRY_CLIENT'] = null;

// Function to initialize Application Insights SDK
function local_appinsights_init_telemetry() {
    global $CFG, $GLOBALS;

    // Check if Application Insights is enabled via environment variables (recommended for App Service)
    $connectionString = getenv('APPLICATIONINSIGHTS_CONNECTION_STRING');

    if (empty($connectionString)) {
        // Fallback: If not found in env, check Moodle config (less ideal for secrets)
        $connectionString = get_config('local_appinsights', 'connectionstring');
    }

    if (!empty($connectionString) && $GLOBALS['APPINSIGHTS_TELEMETRY_CLIENT'] === null) {
        try {
            $telemetryClient = new Telemetry_Client();
            $telemetryClient->getContext()->setInstrumentationKey($connectionString);

            // --- Capture PHP Errors and Exceptions ---
            set_exception_handler(function(Throwable $exception) use ($telemetryClient) {
                // Track exception
                $telemetryClient->trackException($exception);
                $telemetryClient->flush(); // Send immediately for unhandled exceptions

                // Re-enable default exception handler and let Moodle's normal process continue
                restore_exception_handler();
                throw $exception;
            });

            set_error_handler(function($errno, $errstr, $errfile, $errline) use ($telemetryClient) {
                // Filter out non-errors based on current error_reporting level
                if (!(error_reporting() & $errno)) {
                    return false;
                }

                $severity = Telemetry_Context_Data_Severity_Level::INFORMATION;
                switch ($errno) {
                    case E_ERROR:
                    case E_CORE_ERROR:
                    case E_COMPILE_ERROR:
                    case E_USER_ERROR:
                    case E_RECOVERABLE_ERROR:
                        $severity = Telemetry_Context_Data_Severity_Level::CRITICAL;
                        break;
                    case E_WARNING:
                    case E_CORE_WARNING:
                    case E_COMPILE_WARNING:
                    case E_USER_WARNING:
                        $severity = Telemetry_Context_Data_Severity_Level::WARNING;
                        break;
                    case E_NOTICE:
                    case E_USER_NOTICE:
                        $severity = Telemetry_Context_Data_Severity_Level::INFORMATION;
                        break;
                    case E_DEPRECATED:
                    case E_USER_DEPRECATED:
                        $severity = Telemetry_Context_Data_Severity_Level::VERBOSE;
                        break;
                }

                $telemetryClient->trackTrace(
                    "PHP Error ($errno): $errstr in $errfile on line $errline",
                    $severity,
                    ['file' => $errfile, 'line' => $errline, 'errno' => $errno]
                );
                // No flush here, let it queue, unless it's a critical error
                return false; // Let PHP's default error handler continue
            });

            // --- Track Web Requests (simplified) ---
            // This captures the overall request duration and status.
            $requestStartTime = $_SERVER['REQUEST_TIME_FLOAT'] ?? microtime(true);
            $requestUrl = $_SERVER['REQUEST_URI'] ?? 'unknown_url';

            $telemetryClient->trackRequest(
                $requestUrl,
                $requestStartTime,
                0, // Duration will be calculated on flush/shutdown
                200 // Default status code, will be updated on shutdown
            );

            // Store client globally for other Moodle parts if needed
            $GLOBALS['APPINSIGHTS_TELEMETRY_CLIENT'] = $telemetryClient;

            // --- Shutdown Function to Flush Telemetry ---
            register_shutdown_function(function() use ($telemetryClient, $requestStartTime) {
                // Update request duration
                $requestDuration = (microtime(true) - $requestStartTime) * 1000; // in milliseconds
                $currentStatusCode = http_response_code();
                if ($currentStatusCode === false) $currentStatusCode = 200; // Default

                // Note: Updating a tracked request is not directly exposed for duration/status.
                // The PHP SDK is more about tracking *new* items.
                // For a proper request end, you might need to build your own request tracking helper.
                // For simplicity, flushing any queued telemetry.
                $telemetryClient->flush();
            });

        } catch (Exception $e) {
            // This should not break Moodle, only log to standard error
            error_log("Application Insights SDK initialization failed: " . $e->getMessage());
        }
    }
}

// Ensure scoped autoloader is included for the SDK classes
// This path is relative to the Moodle root which is /var/www/html/public
if (file_exists(__DIR__ . '/../../appinsights-scoped/vendor/autoload.php')) {
    require_once __DIR__ . '/../../appinsights-scoped/vendor/autoload.php';
} else {
    // Log to stderr because App Insights isn't initialized yet
    error_log("Application Insights SDK: Composer autoloader (scoped) not found at " . __DIR__ . "/../../appinsights-scoped/vendor/autoload.php");
}

// Call the initialization function early in Moodle's lifecycle
local_appinsights_init_telemetry();

// You can add more specific telemetry if needed by accessing $GLOBALS['APPINSIGHTS_TELEMETRY_CLIENT']
// For example, in a custom activity module or block:
/*
if (isset($GLOBALS['APPINSIGHTS_TELEMETRY_CLIENT']) && $GLOBALS['APPINSIGHTS_TELEMETRY_CLIENT'] instanceof Telemetry_Client) {
    $GLOBALS['APPINSIGHTS_TELEMETRY_CLIENT']->trackEvent('UserVisitedCourse', ['courseid' => $course->id, 'userid' => $USER->id]);
}
*/