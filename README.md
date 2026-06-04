# EventEase Booking System

## Project Overview

EventEase Booking System is an ASP.NET Core MVC web application developed for the CLDV6211 Cloud Development A POE. The system is designed to help booking specialists manage venues, events, and bookings for EventEase.

The application allows users to create, view, update, and delete venue, event, and booking records. The project was developed across multiple phases. Earlier versions used SQL Server LocalDB and Azurite for local development. The final version was deployed to Microsoft Azure using Azure App Service, Azure SQL Database, and Azure Blob Storage.

## Live Website

The deployed EventEase Booking System can be accessed using the following Azure App Service URL:

```text
https://eventeaseweb10433044-bte7g2f4bpa4chgk.centralindia-01.azurewebsites.net
```

## Technologies Used

* ASP.NET Core MVC
* C#
* Entity Framework Core
* SQL Server LocalDB
* Azure SQL Database
* Azure App Service
* Azure Blob Storage
* Azure Storage Emulator / Azurite
* Azure Storage Explorer
* Bootstrap
* HTML, CSS, and Razor Views
* GitHub for source control
* Visual Studio 2022

## Azure Services Used

### Azure App Service

Azure App Service was used to host the ASP.NET Core MVC web application. The application was published from Visual Studio to an existing Azure App Service named:

```text
eventeaseweb10433044
```

### Azure SQL Database

Azure SQL Database was used to store the application data in the cloud. Entity Framework Core migrations were applied to the Azure SQL Database to create the required tables.

The Azure SQL Database used for this project is:

```text
EventeaseDB
```

The main database tables include:

* Venues
* Events
* EventTypes
* Bookings
* __EFMigrationsHistory

### Azure Blob Storage

Azure Blob Storage was used to store uploaded event images. A storage account was created and a blob container was used to store image files uploaded through the application.

The storage account used is:

```text
eventeasestorage10433044
```

The blob container used is:

```text
venue-images
```

When an image is uploaded through the Events section, the image file is stored in Azure Blob Storage and the image URL is saved in the Azure SQL Database.

### Azure Resource Group

All Azure resources for the project were grouped inside one resource group:

```text
rg-eventease-centralindia
```

This made the resources easier to manage and monitor.

## Main Features

### Venue Management

The Venue Management section allows booking specialists to manage venues.

Features include:

* Create venues
* View the venue list
* Edit venue details
* View venue details
* Delete venues
* Prevent deletion of venues that have existing bookings

### Event Management

The Event Management section allows booking specialists to manage events and upload event images.

Features include:

* Create events
* Select an event type
* Link an event to a venue
* Upload event images
* Store uploaded images in Azure Blob Storage
* Display uploaded images on the Events page
* Edit event details
* View event details
* Delete events
* Prevent deletion of events that have existing bookings

### Event Type Management

The system includes an EventType table to categorise events.

Examples of event types include:

* Wedding
* Concert
* Conference
* Religious
* Other

This supports advanced booking filtering by event type.

### Booking Management

The Booking Management section allows booking specialists to create and manage bookings.

Features include:

* Create bookings
* View bookings in a consolidated booking view
* Display Booking ID, Venue name, Event name, Event type, Customer name, Start Date, and End Date
* Edit bookings
* View booking details
* Delete bookings
* Prevent double bookings for the same venue during overlapping date/time periods
* Validate that the booking end date/time is after the start date/time

### Advanced Filtering

The Bookings page includes advanced filtering features to help users find bookings quickly.

Users can filter bookings by:

* Search text
* Booking ID
* Event name
* Venue name
* Customer name
* Event type
* Venue
* Date range
* Venue availability

These filters can be used individually or together.

### Double-Booking Prevention

The system prevents users from booking the same venue for overlapping date and time periods.

If a user tries to create a booking for a venue that is already booked during the selected date range, the system displays an error message and blocks the booking.

### Delete Restrictions

The system prevents deletion of venues and events that are already linked to bookings.

This protects the data relationships in the system and prevents orphaned booking records.

## Local Development

During local development, the application used SQL Server LocalDB and Azurite.

The local database connection string was stored in `appsettings.json`:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EventeaseDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

The local blob storage connection string used Azurite:

```json
"AzureBlobStorage": "UseDevelopmentStorage=true"
```

This allowed the application to be tested locally before being moved to Azure.

## Cloud Deployment

For the final deployment, the application was published to Azure App Service.

The production connection strings were configured in Azure App Service under:

```text
App Service → Environment variables → Connection strings
```

The two main connection strings used were:

```text
DefaultConnection
AzureBlobStorage
```

The local `appsettings.json` file was kept for local development only. Sensitive Azure connection strings and passwords were stored in Azure App Service configuration instead of being hardcoded into the project.

## Testing Completed

The following testing was completed on the deployed Azure website:

* Created venues successfully
* Created events successfully
* Uploaded event images successfully
* Confirmed uploaded images appeared in Azure Blob Storage
* Created bookings successfully
* Tested advanced booking filters
* Tested venue availability checking
* Tested double-booking prevention
* Tested delete restrictions for venues and events with linked bookings
* Confirmed Azure SQL Database tables were created using Entity Framework Core migrations

## Example Test Data

### Venue

```text
Venue Name: Lotus Palace Banquet Hall
Location: Pietermaritzburg
Capacity: 250
```

### Wedding Event

```text
Event Name: Priya and Arjun Wedding
Event Type: Wedding
Venue: Lotus Palace Banquet Hall
```

### Music Concert Event

```text
Event Name: Saffron Nights Music Concert
Event Type: Concert
Venue: Lotus Palace Banquet Hall
```

## Screenshots

The project submission includes screenshots showing:

* Azure Resource Group resources
* Azure App Service overview
* Azure SQL Database tables
* Azure Blob Storage container
* App Service connection strings
* Live website pages
* Event creation with image upload
* Bookings page
* Advanced filtering
* Double-booking prevention

  youtubetink
https://youtu.be/ZrUp1Nk8wuI

## Security Notes

Sensitive information such as database passwords and Azure Storage access keys should not be committed to GitHub.

For the deployed version, connection strings were configured in Azure App Service environment variables.

If a storage key or SQL password is exposed, it should be changed or regenerated in the Azure Portal.

## References

GeeksforGeeks. 2026. Microsoft Azure Tutorial. [Online]. Available at: https://www.geeksforgeeks.org/devops/microsoft-azure/ [Accessed: 4 June 2026].

Microsoft. 2024. Secure access and data in Azure Logic Apps. [Online]. Available at: https://learn.microsoft.com/en-us/azure/logic-apps/logic-apps-securing-a-logic-app [Accessed: 1 June 2026].

Mrzygłód, K. 2022. *Azure for developers: implement rich Azure PaaS ecosystems using containers, serverless services, and storage solutions*. 2nd ed. Birmingham: Packt Publishing.

W3Schools. 2026. What is Cloud Computing? [Online]. Available at: https://www.w3schools.com/aws/aws_cloudessentials_cloudcomputing.php [Accessed: 4 June 2026].
