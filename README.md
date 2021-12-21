# Conversion Dashboard

This program is designed for Netcompany, to provide a dashboard for the Conversion Engine.
It queries, parses and visualized the important log data, which assists the user in data analysis of ongoing and previous conversions.
The software is developed by the P3-Project CS-21-SW-3-05 team.

<!-- INSTALLATION -->
  
 ### Installation 
 To get the program up and running, follow these steps
 
 1. Clone the repository

```sh
git clone https://github.com/mirakst/P3-Project.git
```

 2. Compile the program using any IDE that supports `.NET 6.0`

### Getting Started

 Set up your first profile in the settings menu

`Profile name:` The name of the profile.

`Conversion:` The name of the conversion (displayed in the monitoring controls).

`Data source:` Server name or IP.

`Database:` The database for the monitoring.

`Timeout (sec)` The amount of seconds to try connecting before timeout.


### Using the program

 1. **Monitoring**

Press the ⏵ button to connect to the provided database.
You will be prompted for a username and password - leave blank for windows authentication.
The dashboard will present you with 4 modules. All modules can be opened in separate windows by pressing the detach button <img src="https://github.com/mirakst/P3-Project/blob/dev/DashboardFrontend/Icons/Detach.png" width="20">

`Manager module:` Positioned in the top-left. Shows all managers for the selected execution - Press the detach button, or double click a manager to expand the view with more information and a performance tab, that shows CPU and RAM readings for the manager (if they exist).

`Log module:` Positioned in the top-right. Shows all log messages for the selected execution. Messages are color coded by type, the responsible manager is stored as their tooltip, and they can be filtered by toggling the buttons above the module. The filter button (top-right) allows you to change the shown execution, and show/hide managers by context ID.

`Reconciliation report:` Positioned in the bottom-left. Shows all completed managers with reconciliations for the selected execution. Shown reconciliations can be filtered by toggling the buttons above the module. By default, successful reconciliations are not shown. A description is shown on the righthand side for the selected reconciliation test. This description includes details from the test results, and buttons to copy the SQL queries for the tests, if available.

`Health report:` Positioned in the bottom-right. Shows the CPU, RAM and Network readings for all executions. Duration shown can be modified with the combo box (top-right). The latest reading is shown in text, and the different readings can be found in the different tabs.


 2. **Comparing executions**

By utilizing the detached views of the modules, you can compare the module's data for one execution with another.
Simply detach a window and change the execution for that window to the one you want to compare with, and compare it with the data in the main window.
You could do this in two separate detached windows as well.

 <!-- LICENSE -->
 ## License
 
 Distributed under the GNU General Public License v3.0 License. See `LICENSE` for more information. 


 <!-- CONTACT --> 
 ## Contact 
 
 Project Link: [https://github.com/mirakst/P3-Project](https://github.com/mirakst/P3-Project)
 
 Project group email: <cs-21-sw-3-05@student.aau.dk>
