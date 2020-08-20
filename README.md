# OpenDental_Basic_Plugin
The official example plugin is really old, and while it has some great things in there, I had to comment out a ton to get it to compile.

Here is a new basic plugin in a single file that has the following features:

    - Gets current selected patient PatNum into `patient` variable.
    - Executes database queries
    - Converts results into DataTable
    - Basic Logging to a file (helpful in debugging)

As it stands, when you click the button it shows a box that gives you the total number of patients. 

To use it, add it to the OpenDental Solution, added the references for OpenDental, OpenDentBusiness, and DataConnectionBase. Set the build directory to be the bin\debug directory for the OpenDental project and you should be good.

The only part that annoys me, and surely someone will tell me how to fix it, is that you can't make this plugin a dependency of OpenDental so that it rebuilds every time you click "Start" for debugging, because it creates a circular reference. So I just right click the project and select "Build" before I start the debug session.

Just wanted to give you a head start. It isn't really my intention to continue improving this since each company that wants a plugin will want it to do different things, there isn't a great one size fits all, but once you have a button in Open Dental, database access and logging, you should be pretty good to start.

Written by:
Alan Halls DMD
ReminderDental.com CEO
