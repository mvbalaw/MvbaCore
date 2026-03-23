@ECHO OFF
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0build.ps1" %*
EXIT /B %ERRORLEVEL%