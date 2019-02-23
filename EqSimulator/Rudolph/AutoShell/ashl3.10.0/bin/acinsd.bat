@echo off

rem  Startup script for the acinsd server
rem  This command requires the ASHLHOME environment variable.

rem To use an alternate AutoShell installation, modify the line below.
rem set ASHLHOME=

if "%ASHLHOME%"=="" goto ashlhomeError
:ok2

rem  Check JAVA_HOME
set JAVA_HOME=%ASHLHOME%\..\jre8

if EXIST "%JAVA_HOME%\bin\java.exe" goto ok3
goto javaMissingError

:ok3

rem  Check AutoShell

if EXIST %ASHLHOME%\acinsd goto ok4
goto toolsMissingError

:ok4

rem  Save cwd and settings and start it

setlocal
echo Starting acinsd in directory '%ASHLHOME%\acinsd'
cd /D "%ASHLHOME%\acinsd"

setlocal ENABLEDELAYEDEXPANSION

set CLASSPATH=.

FOR /R .\lib %%G IN (*.jar) DO set CLASSPATH=!CLASSPATH!;%%G
FOR /R ..\java %%G IN (ashlmt*.jar) DO set CLASSPATH=!CLASSPATH!;%%G


"%JAVA_HOME%"\bin\java -cp %CLASSPATH%;log4j.properties com.rudolphtech.acinsd.Acinsd %*

endlocal
goto success

:ashlhomeError
echo ASHLHOME variable must point to an AutoShell installation
goto error

:javahomeError
echo JAVA_HOME environment variable must point to a Java 1.8 or greater JRE
goto error

:javaMissingError
echo java.exe not found at "%JAVA_HOME%\bin"
goto error

:toolsMissingError
echo acinsd subdirectory not found at %ASHLHOME%
goto error

:error  
exit /b 1

:success
exit /b 0
