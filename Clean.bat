@echo off
title Unity Clean Project
color 0A

echo 🛑 Stopping Unity processes...
taskkill /IM Unity.exe /F >nul 2>&1
taskkill /IM Unity*.exe /F >nul 2>&1
taskkill /IM ShaderCompilerWorker.exe /F >nul 2>&1
taskkill /IM AssetImportWorker.exe /F >nul 2>&1

:: Wait a few seconds to release file locks
timeout /t 5 /nobreak >nul

echo 🧹 Cleaning Unity Project...
set "PROJECT_DIR=%cd%"
set FOLDERS=Library Temp Logs Obj

for %%F in (%FOLDERS%) do (
    if exist "%PROJECT_DIR%\%%F" (
        echo Deleting %%F...
        rmdir /S /Q "%PROJECT_DIR%\%%F"
        if errorlevel 1 (
            echo ⚠️ Could not delete %%F. Some files are still in use.
        ) else (
            echo %%F deleted.
        )
    ) else (
        echo %%F not found, skipping.
    )
)

echo.
echo ✅ Clean complete!
pause
