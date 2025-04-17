# Notifications service iterations

## POC scope

As part of the POC integration, the service handled email, sms, and letter requests to asess feasibility of integration with GOV.UK Notify for all notification types supported.
See corresponding swagger API spec and architecture diagram.

## V1 scope

V1 of the integration removed handling of SMS and letter as only email notifications are in scope based on the ECSW portal requirements. It also introduced retry handling using exponential backoff.
