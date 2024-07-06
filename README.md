# Introduction 
This repository contains:
1. The internal lib used by the Fixit service to retrieve, persist & scale all notification operations 
3. The CI/CD terraform-based files, and Azure yaml, allowing the deployment of this service (including of the deployment of Azure Notification Hubs)
4. The RestAPI interface (deployed in a multi-tenant fashion), allowing for the client application to get relevant data
5. The event-driven Azure trigger functions' project, handling any relevant message on listened topics
6. Uses APNS/FCM to send native mobile notifications

# Getting Started
1.	This project contains the rest api, of which the entry point is the program.cs file
2.	There are internal nuget packages that this service uses, of which all can be found under the Fixit "list"
3.	Official releases of this service are held in a private Azure Devops project, and will be migrated here shortly

# Build and Test
1. For each C# project, there exists an equivalent unit tests project, all under the same solution. 
