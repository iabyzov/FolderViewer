# requirements
Visual Studio 2017 Community Edition was used for developing
.NET Core SDK 2.2 should be installed
Node.js 10.16.3

# run a project
Set WebHost as a startup project
Check FolderViewer as a default option in a browser configuration
(IIS Express and Kestrel web server causes incorrect cancellation behavior. From the Internet looks like a bug)
Restore nuget packages for solution (can be done in Visual Studio)
Go to ClientApp in WebHost directory. Execute npm install in bash
Run the startup project in Visual Studio

# general info
this project was based upon a visual studio template. 
This template was refined to a proof-of-concept solution in typical enterprise scenarions.
In case you found a reference which is not related to this assessment please ignore it. 

