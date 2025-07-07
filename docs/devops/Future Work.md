# Future Work

This document outlines future work to be carried out. Items should be crossed out / removed as they are delivered.

## DevOps

### General Work Items

1. Branching and releasing. A branching strategy needs to be decided upon (e.g. Gitflow, Github Flow, trunk-based etc). Once this has been decided, the following will need to be implemented:
   - Automatic creation of builds from candidate release branches on PR.
   - Potential restriction of release? E.g. development environments any build, test / prod environments only release builds.

2. Ensure CI pipeline is run, including tests, as part of PR pre-checks.
   
3. Alerting. There is some basic health check alerting for web apps, but this needs to be expanded upon.

4. Monitoring. Create a dashboard which monitors all of the service components and supports drilling down to add issue resolution.

5. Versioning. Currently, the Docker images are versioned, but the internal components aren't. This may be enough, or we may choose to keep the component files versioned as well.

6. Dependency / security scanning. We need to have a process in place to address the security issues raised from the automatic scanning.
   
7. When a domain name has been established, introducing SSL certificates into Front Door for all environment domain names. Would be good to manage subdomains as per the environment name, e.g. `d01.swip.education.gov.uk` for the d01 dev environment and `swip.education.gov.uk` for the production domain name. 

8. End user friendly deployments. I.e. take site down with unavailable banner, then bring back up. 

9. Consider building cumulative release notes from commits.

10. The workflows don't currently cope with redeploying the same image. (The app version is already correct, so doesn't wait for container restart.) Might want to implement an optional minimum wait time before checking the version endpoint.

11. Secrets rotation.

12. Investigate Microsoft Edge basic auth solutions, possibly involving subdomains and cookies.

### Pre-prod / Prod Work Items

Issues to be addressed for pre-prod / production.

1. Establishing a production container registry, available to pre-prod and prod.
2. Promoting images between dev and production container registries.
3. Setup of Onelogin config with the central Onelogin team.
4. Protected release workflow - only certain groups / individuals can release to pre-prod / prod.
5. Sizing of prod environment.
