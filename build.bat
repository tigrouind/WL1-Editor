@echo off

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /p:Configuration=Release ".\WLEditor.sln" /t:Rebuild
if %ERRORLEVEL% NEQ 0 pause

"%PROGRAMFILES%\7-Zip\7z" a -tzip "WLEditor.zip" ^
 ".\WLEditor\bin\Release\WLEditor.exe" ^
 ".\WLEditor\bin\Release\WLEditor.exe.config" ^
 "-mx=9"
if %ERRORLEVEL% NEQ 0 pause 