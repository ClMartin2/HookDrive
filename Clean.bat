@echo off
title Unity Clean Project
color 0A

echo.
echo ================================
echo   🧹 Cleaning Unity Project...
echo ================================
echo.

:: Store current directory
set PROJECT_DIR=%cd%

:: Folders to delete (no Builds folder)
set FOLDERS=Library Temp Logs Obj

for %%F in (%FOLDERS%) do (
    if exist "%PROJECT_DIR%\%%F" (
        echo Deleting %%F...
        rmdir /S /Q "%PROJECT_DIR%\%%F"
        echo %%F deleted.
    ) else (
        echo %%F not found, skipping.
    )
)

echo.
echo ✅ Clean complete! 
echo.
pause
