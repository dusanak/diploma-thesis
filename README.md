# diploma-thesis
My diploma thesis
This repository contains the codebase for my diploma thesis.

## Prerequisites
 - Docker
 - .NET 6.0
 - Rider or Visual Studio

## Run instructions
1) Run the docker-compose file in the Infrastructure directory to start the database server.
2) In your preferred IDE add a user secret "ClientSecret": "<USER_SECRET_IN_AZURE_ACTIVE_DIRECTORY>" identifying your app registered to a Power BI workspace.
3) Run the server application. If it is not started automatically, run the client application as well. Application should be accessible at https://localhost:7116/.

## More information
It is necessary to register in the application. Email doesn't need to be valid, email confirmation is turned off, anything in the format X@Y.Z will do.

When the application is started, an account with admministration privileges is created. The login information is **admin@admin.cz** for username and **admin1234** for passowrd.
