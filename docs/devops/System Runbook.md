# System Runbook

## Guidelines

### Environments & Deployment

1. New environment creation: The proxy service should be deployed first in a new environment. The auth service deployment uses the proxy service to apply database migrations.

### Authentication

1. Site access: Use this link pattern to clear the saved site authentication credentials from a browser. https://clear:clear@SiteDomain, e.g. https://clear:clear@s205d03-fd-endpoint-web-wa-moodle-primary-b3ayfzapfahtavhp.a01.azurefd.net

### Upgrading

1. Custom Moodle build versions: The available custom Moodle image builds can be viewed by going to the Azure portal, selecting the Azure Container Registry (currently in d01 environment) and then selecting Services -> Repositories -> dfe-digital-swip-digital-service/wa-moodle. Example image labels: 405-20250709.198d868.dev, 500-20250710.2dce568. No `.dev` means the image was built from the `main` branch.

2. Upgrading Moodle: The Moodle build workflow supports building from Moodle branches `405` and `500`. Select the branch you want to build from, or edit the workflow to support new branches. The resulting image can be deployed to any environment instance, though attempting to downgrade versions will result in deployment failure.
   
3. Upgrading GovUK Moodle theme: The Moodle build workflow currently supports theme version `v0.0.3`. More versions can be added to the drop-down as they are released. The selected theme will be built into the Moodle image and can then be deployed to an environment instance.

4. Downgrading: If you deploy a higher version to a Moodle environment then attempt to redeploy a lower version, the Moodle website will fail. If data loss is not an issue, simply delete the Moodle database from the PostrgeSQL flexible server in the Azure portal. (Settings -> Database, then for example `s205d01_db_moodle_primary` dependent on the environment.) Once this is done, rerun the Terraform Deploy workflow with Plan & Apply selected. An empty Moodle database will be rebuilt and then the lower version Moodle image can be redeployed to the environment.

## Current Issues

1. New environment creation: Currently when adding a new environment, e.g. t01, the Terraform workflow must be run twice - the first time will fail. This is most likely due to timing considerations when creating and accessing secrets. Sufficiently low priority not to be fixed yet.

2. Site access: Microsoft Edge can be configured to block Basic Auth headers. Some of our end users who used Edge were affected by this and were unable to progress through basic authentication on the end user Moodle site. The only current workaround is to use another browser.
