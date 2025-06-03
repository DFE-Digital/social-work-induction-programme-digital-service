# Github Actions Notes

## Commit References
Prefer commit references to versions when using a Github Actions workflow. E.g. instead of this:

```
steps:
    - name: Checkout repository
      uses: actions/checkout@v4
```

Use this:

```
    steps:
    - name: Checkout repository
      uses: actions/checkout@11bd71901bbe5b1630ceea73d27597364c9af683  # v4
```

This mitigates against upstream supplier attacks which could potentially introduce malicious workflow code via re-tagging. Note the comment against the commit hash which helps us quickly assess the current version without having to return to the relevant repository.

## Github Actions Repositories

If a new action is introduced, update the list below so the latest versions can be assessed more easily.

- azure/login - https://github.com/Azure/login/tags
- actions/checkout - https://github.com/actions/checkout/tags
- actions/setup-just - https://github.com/extractions/setup-just/tags
- actions/setup-python - https://github.com/actions/setup-python/tags
- bridgecrewio/checkov-action - https://github.com/bridgecrewio/checkov-action/tags
- azure/webapps-deploy - https://github.com/Azure/webapps-deploy/tags
- hashicorp/setup-terraform - https://github.com/hashicorp/setup-terraform/tags
- Azure/appservice-settings - https://github.com/Azure/appservice-settings/tags
- marocchino/validate-dependabot - https://github.com/marocchino/validate-dependabot/tags
