# FlowEngine
Flow Engine backend business logic controller. The main goal being to make code deployments easier to a Linux server, All my development is done on a Windows desktop, but the actual production system is Linux. I want to use it to replace my Python REST API.
Each execution unit is called a "Flow", a flow contains "Steps" which are basically functions, each step has "Input" and "Output" connection points which are connected via "Links"

#Components
## Engine
The engine will run on Linux or Windows. There is no user interface for the engine (except some command line parameters). The engine just loads "Flows" controls the plugins which do all the work.

## Plugins
The plugins do all the actual work. Plugins instruct the engine to start flows based on events. Each plugin has unique settings that can be configured, for instance the Http plugin has settings that control which IP and port to listen for inbound requests on.

### Http plugin
Listens for incoming HTTP requests and will start flows based on the path/route in the request.

### Database plugin

### FlowCore plugin
Basic flow controls

### Log plugin
Log flow events into a log file

### Sessions (Future plugin)
Control users sessions with logins passwords and encryption, will handle all the normal stuff (login, logout, forgot password / password reset, logout all sessions, ...)

## Designer 
The designer is a Windows only application that is the user interface for the Flow Engine. It lets you design and build flows that will run inside the engine.

## Work in progress, early stages 
This project is a work in progress. The engine works, but can't really do anything yet. The only steps that works is Flow.Sleeep, Flow.Run, & Flow.RunAsync, which were my testing steps. The designer can load and save flows locally but not send them to the server yet. The graphics in the designer are place holders only for now. They will be updated at some point in the future.

## Future features
Many of the below features are fleshed out in the code, but they are far from complete
* Run on both Linux and Windows (.Net Core)
* Users with Security profiles (Admin, developer, readonly, ...)
* Engine running type profiles (Production, QA, development, ...)
* Upload flows to server and have them update / added in real time, no reboot required
* Breakpoints
* Trace log
* Jump points (a way to link to other areas of a flow)
* Comments in flow
* Compile flows, check flows for errors (missing connections, missing parameters, ...)
* Layouts, save, load