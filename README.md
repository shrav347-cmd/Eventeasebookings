# EventEase Booking System

## Project Overview

EventEase Booking System is an ASP.NET Core MVC web application developed for the CLDV6211 Cloud Development A POE. The system is designed to help booking specialists manage venues, events, and bookings for EventEase.

The application allows users to create, view, update, and delete venue, event, and booking records. In Part 2, the system was enhanced with local cloud storage using Azurite, image upload functionality, booking validation, deletion restrictions, and search functionality.

## Technologies Used

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server LocalDB
- Azure Storage Emulator / Azurite
- Azure Storage Explorer
- Bootstrap
- HTML, CSS, and Razor Views
- GitHub for source control

## Main Features

### Venue Management

- Create venues
- View venue list
- Edit venue details
- View venue details
- Delete venues
- Prevent deletion of venues that have existing bookings

### Event Management

- Create events
- Upload event images
- Store uploaded images in Azurite Blob Storage
- Display uploaded images on the Events page
- Edit event details
- View event details
- Delete events
- Prevent deletion of events that have existing bookings

### Booking Management

- Create bookings
- View bookings in a consolidated booking view
- Display Booking ID, Venue name, Event name, Customer name, Start Date, and End Date
- Edit bookings
- Delete bookings
- Prevent double bookings for the same venue during overlapping date/time periods
- Validate that the booking end date/time is after the start date/time

### Search Functionality

The Bookings page includes a search feature that allows booking specialists to search by:

- Booking ID
- Event name
- Venue name
- Customer name

### Local Blob Storage

For Part 2, image storage was implemented using Azurite to emulate Azure Blob Storage locally.

The local blob container used is:

```text
venue-images
