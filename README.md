# Tenki

Tenki is a simple weather displayer for the terminal written in C# using Spectre.Console framework.

![preview](https://github.com/Antoniowski/tenki/blob/main/tenki_preview.png?raw=true)


[open-meteo.com](https://open-meteo.com/en/docs) is used as weather API. The version used is the "non-commercial" one, so it is limited to 10000 call 
a day. Same story for the geocoding API.

## Installation
For now, there is no proper installation method. The only way is to compile the program using one of the following command:
```
dotnet build
```
or
```
dotnet publish -c Release --runtime linux-x64
```

this will create a folder in the same path of the project containing the executable.

N.B. the runtime parameter is used to specify which executable will be created (win-x64 if you are on Windows, linux-x64 for linux).
You can also add a ```--self-contained``` parameter to the command to create a "standalone" version of the program that will run even if
.NET is not installed on the PC.

## How to use
After installing you need to just open the program from the terminal.
The first time the program will be executed, it will let the user choose the city. After that, a tenki.conf file will be created in the user's
.config directory.
Deleting the .conf file will reset the application.

## Known issues
- Sometimes the weather get method is really slow (up to 2 minutes). As consequence, the weather update or simply the boot of the application could be 
slow.  
- Temperature is available only in Celsius for now.

