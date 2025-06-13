<?php
// scoper.inc.php

require_once '/root/.composer/vendor/autoload.php';

use PhpScoper\Configuration\Option\ExcludeNamespace;
use PhpScoper\Finder\ComposerFinder; // Ensure this is used
use PhpScoper\PhpScoper; // This might not be needed if not used directly, but good to include relevant Scoper classes

return [
    'prefix' => 'MoodleAppInsights', // Your chosen prefix
    'finders' => [
        ComposerFinder::create(__DIR__ . '/appinsights-isolated-vendor'),
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