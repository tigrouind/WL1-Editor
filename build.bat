@echo off

rd /s/q ".\WLEditor\bin\Release\net9.0-windows\"

dotnet build -c Release ".\WLEditor.sln"
if %ERRORLEVEL% NEQ 0 pause

"%PROGRAMFILES%\7-Zip\7z" a -tzip "WLEditor.zip" ^
 ".\WLEditor\bin\Release\net9.0-windows\*" ^
 "-mx=9"
if %ERRORLEVEL% NEQ 0 pause 
