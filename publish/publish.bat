@echo off
setlocal EnableExtensions

set "SCRIPT_DIR=%~dp0"
set "ROOT=%SCRIPT_DIR%.."
set "APIKEY_FILE=%SCRIPT_DIR%apikey"
set "PACKAGE_OUT=%SCRIPT_DIR%packages"
set "BUILD_ONLY=0"
set "NUGET_SOURCE=https://api.nuget.org/v3/index.json"
set "APICLIENT_PROJ=%ROOT%\src\MyLab.ApiClient\MyLab.ApiClient.csproj"
set "APICLIENT_TEST_PROJ=%ROOT%\src\MyLab.ApiClient.Test\MyLab.ApiClient.Test.csproj"

:parse_args
if "%~1"=="" goto args_done
if /I "%~1"=="build-only" set "BUILD_ONLY=1"
if /I "%~1"=="--build-only" set "BUILD_ONLY=1"
if /I "%~1"=="-build-only" set "BUILD_ONLY=1"
shift
goto parse_args
:args_done

echo === Build MyLab.ApiClient (Release) ===
dotnet build "%APICLIENT_PROJ%" -c Release --nologo
if errorlevel 1 goto fail

echo === Build MyLab.ApiClient.Test (Release) ===
dotnet build "%APICLIENT_TEST_PROJ%" -c Release --nologo
if errorlevel 1 goto fail

echo === Pack NuGet packages ===
if exist "%PACKAGE_OUT%" rmdir /s /q "%PACKAGE_OUT%"
mkdir "%PACKAGE_OUT%"

dotnet pack "%APICLIENT_PROJ%" -c Release --no-build --nologo -o "%PACKAGE_OUT%"
if errorlevel 1 goto fail

dotnet pack "%APICLIENT_TEST_PROJ%" -c Release --no-build --nologo -o "%PACKAGE_OUT%"
if errorlevel 1 goto fail

echo Packages:
dir /b "%PACKAGE_OUT%\*.nupkg"

if "%BUILD_ONLY%"=="1" (
    echo Build-only: packages created, publish skipped.
    goto done
)

if not exist "%APIKEY_FILE%" (
    echo ERROR: NuGet API key file not found: "%APIKEY_FILE%"
    exit /b 1
)

set "APIKEY="
for /f "usebackq delims=" %%A in ("%APIKEY_FILE%") do (
    set "APIKEY=%%A"
    goto apikey_loaded
)
:apikey_loaded

if not defined APIKEY (
    echo ERROR: NuGet API key file is empty: "%APIKEY_FILE%"
    exit /b 1
)

echo === Push packages to nuget.org ===
dotnet nuget push "%PACKAGE_OUT%\*.nupkg" --api-key "%APIKEY%" --source "%NUGET_SOURCE%" --skip-duplicate
if errorlevel 1 goto fail

echo Publish completed.
goto done

:fail
echo FAILED.
exit /b 1

:done
endlocal
exit /b 0
