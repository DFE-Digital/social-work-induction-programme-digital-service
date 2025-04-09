workspace "Social Work Induction Programme" "A high level architectural overview of the planned Social Work Induction Programme website" {

    !identifiers hierarchical

    model {
        socialWorker = person "Newly Qualified Social Worker" "A newly qualified social worker wanting to improve their knowledge and skills"
        userAssessor = person "Assessor" "A person that creates and performs assessments"
        userDfEManager = person "DfE Manager" "A person that performs local authority administration"
        userCoordinator = person "Coordinator" "A person that performs user administration"
        
        # External
        onelogin = softwareSystem "GOV.UK One Login" "One Login - GOV.UK Authentication System" {
            tags "External, OneLogin"
            socialWorker -> this "authenticates with"
            userAssessor -> this "authenticates with"
            userDfEManager -> this "authenticates with"
            userCoordinator -> this "authenticates with"
        }

        govNotify = softwareSystem "GOV.UK Notify" "GOV.UK Notify - A UK government service that sends emails, text messages and letters to users" {
            tags "External, GovNotify"
            this -> socialWorker "receives system generated communication"
            this -> userAssessor "receives system generated communication"
            this -> userDfEManager "receives system generated communication"
            this -> userCoordinator "receives system generated communication"
        }

        # Context
        group "Social Work Induction Programme" {
            swipAuthentication = softwareSystem "SWIP Authentication Service" {
                description "SWIP Authentication Service"
                userAuthentication = container "User Authentication" {
                    this -> onelogin "verifies identity"
                    tags "authentication service"
                    description "Authentication Service"
                    technology "C#"
                }
            }

            swipUserManagement = softwareSystem "SWIP User Management" {
                description "SWIP User Management System"
                userManageFE = container "User Management Application" {
                    this -> swipAuthentication.userAuthentication "authentication calls"
                    userDfEManager -> this "performs local authority administration"
                    userCoordinator -> this "performs user administration"
                    tags "web application"
                    description "User Management"
                    technology "C#"
                }
                db = container "Database Schema" {
                    userManageFE -> this "Reads and writes data"
                    technology "PostgreSQL"
                    tags "Microsoft Azure - Azure Database PostgreSQL Server, Database"
                }
                notifications = container "Notifications Service" {
                    userManageFE -> this "uses"
                    this -> govNotify "send notifications to users"
                    technology "C#"
                    tags "Microsoft Azure - Function Apps"
                }
            }

            moodle = softwareSystem "Moodle" {
                description "Moodle Learning Management System"
                moodleService = container "Moodle Application" {
                    socialWorker -> this "completes assigments and tracks their progress"
                    userAssessor -> this "assesses assignments"
                    this -> onelogin "verifies identity"
                    swipUserManagement.userManageFE -> this "administers user on"
                    this -> govNotify "sends communication"
                    tags "moodle"
                    technology "PHP"
                }
                db = container "Database Schema" {
                    moodleService -> this "Reads and writes data"
                    technology "PostgreSQL"
                    tags "Microsoft Azure - Azure Database PostgreSQL Server, Database"
                }
            }
        }

        # Deployment Diagram
        group deployment {
            live = deploymentEnvironment "Cloud Infrastructure Platform" {
                azure = deploymentNode "Microsoft Azure" {
                    tags "Microsoft Azure - Azure Deployment Environments"
                
                    region = deploymentNode "West Europe" {
                        tags "Microsoft Azure - Region Management"
                
                        dns = infrastructureNode "DNS Router" {
                            technology "Azure DNS"
                            description "Routes incoming requests based upon domain name"
                            tags "Microsoft Azure - DNS Zones"
                        }
                        gateway = infrastructureNode "Azure Front Door" {
                            technology "Azure Front Door and Firewalls"
                            description "Automatically distributes and secures incoming application traffic"
                            tags "Microsoft Azure - Firewalls"
                            dns -> this "Forwards requests to" "HTTPS"
                        }
                        moodleWeb = infrastructureNode "Autoscaling Web App - Moodle" {
                            technology "Azure Application Service Plan"
                            description "Compute Service"
                            tags "Microsoft Azure - App Service Plans"
                            gateway -> this "forwards requests to" "HTTPS"
                        }
                        userManagementAppPlan = infrastructureNode "Autoscaling Web App - User Management" {
                            technology "Azure Application Service Plan"
                            description "Compute Service"
                            tags "Microsoft Azure - App Service Plans"
                            gateway -> this "forwards requests to" "HTTPS"
                        }
                        postgres = infrastructureNode "Azure Database for PostgreSQL Flexibile Server" {
                            technology "PostgreSQL"
                            description "Database"
                            tags "Microsoft Azure - Azure Database PostgreSQL Server"
                            moodleWeb -> this "reads and writes data"
                            userManagementAppPlan -> this "reads and writes data"
                        }
                        keyvault = infrastructureNode "Azure Key Vault" {
                            tags "Microsoft Azure - Key Vaults"
                            moodleWeb -> this "reads and stores secrets"
                            userManagementAppPlan -> this "reads and stores secrets"
                        }
                        containerRegistry = infrastructureNode "Azure Container Registry" {
                            tags "Microsoft Azure - Container Registries"
                            moodleWeb -> this "pulls container images from"
                            userManagementAppPlan -> this "pulls container images from"
                        }
                    }
                }
            }
        }    
    }

    views {
        themes "https://static.structurizr.com/themes/microsoft-azure-2023.01.24/theme.json"

        styles {
            element "Person" {
                shape person
                background #6258b3
                color white
            }

            element "Software System" {
                color white
                background #5187b0
            }

            element "web application" {
                color white
                background #00008B
            }

            element "moodle" {
                color white
                background #e4973f
            }            

            element "Database" {
                shape cylinder
                color white
                background #3c2ade
            }

            element "OneLogin" {
                icon "./icons/govuk-logo.png"
            }

            element "GovNotify" {
                icon "./icons/govuk-logo.png"
            }

            element "External" {
                background #a9a9a9
            }
        }        
    }
}