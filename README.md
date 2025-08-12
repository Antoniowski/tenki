# Tenki

Tenki is a simple weather displayer for the terminal written in C# using Spectre.Console framework.

[!alt text](https://github.com/Antoniowski/tenki/blob/main/tenki_preview.png?raw=true)


Open-meteo.com  is used as weather API. The version used is the "non-commercial" one, so it is limited to 10000 call 
a day. Same story for the geocoding API that uses geocoding.open-meteo.com.

## Installation
For now, there is no proper installation method. The only way is to compile the program using the following command:
```
dotnet build
```
this will create a folder in the same path of the project containing the executable.

## How to use
After installing you need to just open the program from the terminal.
The first time the program will be executed, it will let the user choose the city. After that, a tenki.conf file will be created in the user's
.config directory.
Deleting the .conf file will reset the application.

## Known issues
- Sometimes the weather get method is really slow (up to 2 minutes). As consequence, the weather update or simply the boot of the application could be 
slow.  
- Temperature is available only in Celsius for now.

