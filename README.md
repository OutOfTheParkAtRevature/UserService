# User Service

## Description
This service manages users and access-level roles. Users are assigned roles and each role has access to different functionalities of the application. The Admin, League Manager, or Head Coach can approve user roles. Admin and League Managers can edit or delete any user. Head Coaches can only edit or delete users on their team. Parents can only edit or delete their own child, or Player.

## Functionality
* Creates users
* Assigns a role to users
* Deletes users
* Edits users

## Getting Started
1. Clone this repo to your local machine:
```bash
  git clone https://github.com/OutOfTheParkAtRevature/UserService.git
```
2. To create and connect first branch to remote repository branch:
```bash
  git push --set-upstream origin
```
3. Use IDE like Visual Studio to run application. This will bring Swagger up in the browser and allow you to test the service.

## Links
This Repository is part of an application designed with a Microservice Architecture. The other parts of the application are listed below

* Album Service: https://github.com/OutOfTheParkAtRevature/AlbumService
* Calendar Service: https://github.com/OutOfTheParkAtRevature/CalendarService
* Equipment Service: https://github.com/OutOfTheParkAtRevature/EquipmentService
* Frontend: https://github.com/OutOfTheParkAtRevature/Frontend
* Gateway Service: https://github.com/OutOfTheParkAtRevature/Gateway
* League Service: https://github.com/OutOfTheParkAtRevature/LeagueService
* Message Service: https://github.com/OutOfTheParkAtRevature/MessageService
* News Service: https://github.com/OutOfTheParkAtRevature/NewsService
* Notification Service: https://github.com/OutOfTheParkAtRevature/NotificationService
* Playbook Service: https://github.com/OutOfTheParkAtRevature/PlaybookService
* Season Service: https://github.com/OutOfTheParkAtRevature/SeasonService
* Stat Service: https://github.com/OutOfTheParkAtRevature/StatService.git
