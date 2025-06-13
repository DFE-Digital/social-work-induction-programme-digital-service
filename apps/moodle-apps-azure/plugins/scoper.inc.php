<?php
// scoper.inc.php

require_once '/root/.composer/vendor/autoload.php';

return [
    'prefix' => 'MoodleAppInsights', // Your chosen prefix
    'finders' => [
        new \PhpScoper\Finder\ComposerFinder::create(__DIR__ . '/appinsights-isolated-vendor'),
    ],
    'exclude-namespaces' => [
        // If the SDK itself uses the original Guzzle classes directly (unlikely)
        // then you might need to exclude Guzzle from being prefixed inside the SDK's own code
        // Example: 'GuzzleHttp',
    ],
    'patch-ers' => [
        function (string $filePath, string $prefix, string $contents): string {
            // For global functions or if there's any hardcoded string
            // This is more advanced, might not be needed for App Insights
            return str_replace('new GuzzleHttp', 'new ' . $prefix . '\\GuzzleHttp', $contents);
        },
    ],
];