@ECHO OFF
pushd "%~dp0"
ECHO.
ECHO.
ECHO.
ECHO This script deletes all temporary build files in their
ECHO corresponding BIN and OBJ Folder contained in the following projects
ECHO.
ECHO Apps\ThemedDemo
ECHO HL
ECHO TextEditLib
ECHO.
ECHO Components\ServiceLocator
ECHO Components\Settings\Settings
ECHO Components\Settings\SettingsModel
ECHO.
REM Ask the user if hes really sure to continue beyond this point XXXXXXXX
set /p choice=Are you sure to continue (Y/N)?
if not '%choice%'=='Y' Goto EndOfBatch
REM Script does not continue unless user types 'Y' in upper case letter
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
RMDIR /S /Q .\.vs

ECHO.
ECHO Deleting BIN and OBJ Folders in Apps\ThemedDemo
ECHO.
RMDIR /S /Q .\Apps\ThemedDemo\bin
RMDIR /S /Q .\Apps\ThemedDemo\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in HL
ECHO.
RMDIR /S /Q .\HL\bin
RMDIR /S /Q .\HL\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in TextEditLib
ECHO.
RMDIR /S /Q .\TextEditLib\bin
RMDIR /S /Q .\TextEditLib\obj
ECHO.


ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
ECHO.
ECHO Deleting BIN and OBJ Folders in Components\ServiceLocator
ECHO.
RMDIR /S /Q .\Components\ServiceLocator\bin
RMDIR /S /Q .\Components\ServiceLocator\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in Components\Settings\Settings
ECHO.
RMDIR /S /Q .\Components\Settings\Settings\bin
RMDIR /S /Q .\Components\Settings\Settings\obj
ECHO.

ECHO.
ECHO Deleting BIN and OBJ Folders in Components\Settings\SettingsModel
ECHO.
RMDIR /S /Q .\Components\Settings\SettingsModel\bin
RMDIR /S /Q .\Components\Settings\SettingsModel\obj
ECHO.

ECHO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

PAUSE

:EndOfBatch
