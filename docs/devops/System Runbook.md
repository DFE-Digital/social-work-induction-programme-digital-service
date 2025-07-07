# System Runbook

## Guidelines

| Topic | Guideline | 
| -------- | ------- | 
| New environment creation | The proxy service should be deployed first in a new environment. The auth service deployment uses the proxy service to apply database migrations. |
| Site access / basic auth | Use this link pattern to clear the saved site basic authentication credentials from a browser. https://clear:clear@SiteDomain, e.g. https://clear:clear@s205d03-fd-endpoint-web-wa-moodle-primary-b3ayfzapfahtavhp.a01.azurefd.net |
| Upgrading Moodle | The Moodle build workflow supports building from Moodle branches `405` and `500`. Select the branch you want to build from, or edit the workflow to support new branches. The resulting image can be deployed to any environment instance, though attempting to downgrade versions will result in deployment failure. |
| Upgrading GovUK Moodle theme | The Moodle build workflow currently supports theme version `v0.0.3`. More versions can be added to the drop-down as they are released. The selected theme will be built into the Moodle image and can then be deployed to an environment instance. |

## Current Issues

| Topic | Issue | 
| -------- | ------- | 
| New environment creation | Currently when adding a new environment, e.g. t01, the Terraform workflow must be run twice - the first time will fail. This is most likely due to timing considerations when creating and accessing secrets. Sufficiently low priority not to be fixed yet. |
| Site access | Microsoft Edge can be configured to block Basic Auth headers. Some of our end users who used Edge were affected by this and were unable to progress through basic authentication on the end user Moodle site. The only current workaround is to use another browser. |
