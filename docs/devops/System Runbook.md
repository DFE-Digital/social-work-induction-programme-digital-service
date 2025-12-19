# System Runbook

## Guidelines

### Environments & Deployment

1. New environment creation: The proxy service should be deployed first in a new environment. The auth service deployment uses the proxy service to apply database migrations.

### Authentication

1. Site access: Use this link pattern to clear the saved site authentication credentials from a browser. https://clear:clear@SiteDomain, e.g. https://clear:clear@s205d03-fd-endpoint-web-wa-auth-service-b3ayfzapfahtavhp.a01.azurefd.net

## Current Issues

1. New environment creation: Currently when adding a new environment, e.g. t01, the Terraform workflow must be run twice - the first time will fail. This is most likely due to timing considerations when creating and accessing secrets. Sufficiently low priority not to be fixed yet.
