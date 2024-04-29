@echo off
setlocal enableextensions enabledelayedexpansion

:: Define the base directory relative to this script
set "baseDir=%~dp0"

:: Define the paths of the folders to check relative to baseDir
set "folder1=%baseDir%input"
set "folder2=%baseDir%output"
set "folder3=%baseDir%temp"

:: Loop through each folder
for %%f in ("%folder1%" "%folder2%" "%folder3%") do (
    echo Checking %%~f...

    :: Extract the last folder name from the path
    for %%i in ("%%~f") do set "lastFolderName=%%~nxi"

    set "count=0"
    for /f %%a in ('dir "%%~f" /b /a ^| find /v /c "^^"') do set "count=%%a"

    if "!count!"=="0" (
        echo Folder %%~f is empty.
    ) else (
        echo Folder %%~f is not empty.
        set /p "proceed=Do you want to delete all contents in !lastFolderName! (Y/N, default=Y)? "
        
        :: Check if input is empty and assume Yes
        if "!proceed!"=="" set "proceed=y"
        
        if /i "!proceed!"=="y" (
            echo Proceeding with cleanup...
            del /q "%%~f\*.*"
            for /d %%i in ("%%~f\*") do rmdir /s /q "%%i"
			    echo ************************Cleanup complete**********************************
                echo Folder : !lastFolderName!
				echo **************************************************************************
            ) else (
				echo *************************Cleanup canceled*********************************
                echo Folder : !lastFolderName!
				echo **************************************************************************
        )
    )
)

echo.
echo Cleanup process completed.
pause
endlocal
