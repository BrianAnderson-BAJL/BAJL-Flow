# BAJL Flow
BAJL Flow Engine is a backend visual business logic controller. The main goal being to make code deployments easier to a Linux server. All our development is done on a Windows desktop, but the actual production system is Linux. We want to use it to replace our Python REST API.
New Flows (executing code paths) can be deployed to the server without interruption of existing flows. Updates to existing flows can also be performed all without restarting the service or server.

Each execution unit is called a "Flow", a flow contains "Steps" which are basically functions, each step has "Input" and "Output" connection points which are connected via "Links"
# Use cases
* BAJL is using the BAJL Flow Engine as a standalone backend application for a REST API performing CRUD (Create, Read, Update, Delete) operations with a Maria database (previously known as MySql before Oracle purchased it)
* Could be integrated into another .Net application to do almost anything. The core is a .Net Assembly.
* Plugins could be created to do almost anything. Plugins are just .Net assemblies that inherit from the FlowEngineCore.Plugin class.
# Very simple flow (graphics are place holders)
![A very simple flow](https://github.com/BrianAnderson-BAJL/BAJL-Flow/blob/main/ScreenShots/BasicFlow.png)
# Components
The pieces that make the BAJL Flow Engine work!

## Engine
The engine runs on Linux or Windows. There is no user interface for the engine (except for some command line parameters). The engine just loads "Flows" and controls the plugins which do all the work.

## Plugins
The plugins do all the actual work. Plugins instruct the engine to start flows based on events. Each plugin has unique settings that can be configured, for instance the Http plugin has settings that control which IP and port to listen for inbound requests on.
Below are the plugins that exist or are currently planned to be created. New plugins can be added, they are just .Net assemblies that inherit from the Core.Plugin class.

### Http plugin
Listens for incoming HTTP requests and will start flows based on the path/route in the request.

### Database plugin
Perform normal database operations, INSERT, SELECT, UPDATE, DELETE, ...

### FlowCore plugin
Basic flow controls.
#### Start sub flows either synchronously or asynchronously
#### Core steps, Start, Stop, ...

### Email plugin
Send and receive emails. Load email templates from storage and and send customized emails to users.

### Log plugin
Log flow events into a log file, includes archiving, rolling logs when they hit a size threshold, & deleting old logs.

### Sessions plugin
Control users sessions with logins passwords and encryption, will handle all the normal stuff (login, logout, forgot password / password reset, logout all sessions, ...)

### Validation plugin
Will perform any required validation, currently email, phone numbers, and flow variables and json/xml elements exist in request.

### Timer plugin (planned)
Start a flow every X seconds or at a specific time

## Designer 
The designer is a Windows only application that is the user interface for the Flow Engine. It lets you design and build flows that will run inside the engine.

# Work in progress, early stages 
This project is a work in progress. It is currently being used as a REST API for the backend of another project that is in development. The engine works and the included plugins have many functions that work. The designer can load and save flows locally and to the server. The graphics in the designer are place holders only for now. They will be updated at some point in the future.

# Features
Many of the below features are fleshed out in the code
* Run on both Linux and Windows (.Net Core 6)
* Users with Security profiles (Admin, developer, readonly, ...)
* Engine running type profiles (Production, QA, Development, ...)
* Upload flows to server and have them update / added in real time, no reboot/restart required
* Trace log of code performance and inspect variables after each step/function
* Comments in flows
* Designer Layouts. You can layout your desktop however you want and it will remember next time you start it.
* Statistics regarding flows (Counts, avg time to execute, longest flow runtime, shortest flow runtime, ...)

# Future features
These are things that will be included sometime in the future
* Support multiple database sources (currently only supports a single MySql/MariaDb Database)
* Jump points (a way to link to other areas of a flow)
* Compile flows, check flows for errors (missing connections, missing parameters, ...)
