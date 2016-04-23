This document contains the description of this project.

Project Name: WinHost
Project Type: Executable
Project Default Namespace: XecMe
Project Output : WinHost.exe
Project References: XecMe.Core.dll
Signed Assembly : No
GAC-able : No


Description:
This project provides a hosting environment for Windows Services and Batch Job. To make it a Windows Service, 
a class implementing the interface XecMe.Core.Services.IService should be registered with 
Windows SCM as service. Once registered, the calls from SCM are delegated to the class registered for this service.


Host class:
Host class is the extry point for this executable. This class facilitates Windows service installation and run as 
batch job based on the command line parameters passed

Installer class:
This class installs the this assembly and the given parameter as Windows Service