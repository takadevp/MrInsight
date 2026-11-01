@echo off
setlocal

rem Publish MrInsight as a single self-contained exe into app\
set "ROOT=%~dp0"
set "OUTPUT=%ROOT%app"

dotnet publish "%ROOT%MrInsight\MrInsight.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none -o "%OUTPUT%"
if errorlevel 1 (
	echo Build failed.
	pause
	exit /b 1
)

rem Keep an existing settings.json so that rebuilding does not erase the access token
if not exist "%OUTPUT%\settings.json" (
	copy "%ROOT%MrInsight\settings.example.json" "%OUTPUT%\settings.json" >nul
	echo Created %OUTPUT%\settings.json. Fill in GitLabUrl and GitLabAccessToken.
)

echo Built %OUTPUT%\MrInsight.exe
pause
