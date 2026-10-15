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
set "VERSION="
set "SUFFIX="
set "HAS_SUFFIX=0"

:parse_args
if "%~1"=="" goto args_done

if /I "%~1"=="build-only" goto set_build_only
if /I "%~1"=="--build-only" goto set_build_only
if /I "%~1"=="-build-only" goto set_build_only

if /I "%~1"=="suffix" goto take_suffix
if /I "%~1"=="-suffix" goto take_suffix
if /I "%~1"=="--suffix" goto take_suffix

set "ARG=%~1"
if "%ARG:~0,1%"=="-" goto unknown_arg
if "%ARG:~0,1%"=="/" goto unknown_arg
if defined VERSION goto unknown_arg

set "VERSION=%~1"
shift
goto parse_args

:set_build_only
set "BUILD_ONLY=1"
shift
goto parse_args

:take_suffix
if "%HAS_SUFFIX%"=="1" (
    echo ERROR: Parameter suffix is specified more than once.
    call :usage
    exit /b 1
)
shift
if "%~1"=="" goto suffix_missing
set "ARG=%~1"
if "%ARG:~0,1%"=="-" goto suffix_missing
if "%ARG%"=="" goto suffix_missing
set "SUFFIX=%~1"
set "HAS_SUFFIX=1"
shift
goto parse_args

:suffix_missing
echo ERROR: Parameter suffix must be a non-empty string.
call :usage
exit /b 1

:unknown_arg
echo ERROR: Unknown argument: "%~1"
call :usage
exit /b 1

:args_done
if not defined VERSION (
    echo ERROR: Version number is required. Expected format xx.xx.xx.
    call :usage
    exit /b 1
)

call :validate_version "%VERSION%"
if errorlevel 1 (
    echo ERROR: Version "%VERSION%" does not match xx.xx.xx, where xx is a number.
    call :usage
    exit /b 1
)

set "PACKAGE_VERSION=%VERSION%"
if "%HAS_SUFFIX%"=="1" set "PACKAGE_VERSION=%PACKAGE_VERSION%-%SUFFIX%"

for /f %%I in ('powershell -NoProfile -NonInteractive -Command "Get-Date -Format ddMM"') do set "DT=%%I"
call :is_digits4 "%DT%"
if errorlevel 1 (
    echo ERROR: Failed to resolve the current day and month.
    exit /b 1
)

set "LIBRARY_VERSION=%VERSION%.%DT%"

echo Package version: %PACKAGE_VERSION%
echo Library version: %LIBRARY_VERSION%

echo === Build MyLab.ApiClient (Release) ===
dotnet build "%APICLIENT_PROJ%" -c Release --nologo -p:Version="%PACKAGE_VERSION%" -p:AssemblyVersion="%LIBRARY_VERSION%" -p:FileVersion="%LIBRARY_VERSION%"
if errorlevel 1 goto fail

echo === Build MyLab.ApiClient.Test (Release) ===
dotnet build "%APICLIENT_TEST_PROJ%" -c Release --nologo -p:Version="%PACKAGE_VERSION%" -p:AssemblyVersion="%LIBRARY_VERSION%" -p:FileVersion="%LIBRARY_VERSION%"
if errorlevel 1 goto fail

echo === Pack NuGet packages ===
if exist "%PACKAGE_OUT%" rmdir /s /q "%PACKAGE_OUT%"
mkdir "%PACKAGE_OUT%"

dotnet pack "%APICLIENT_PROJ%" -c Release --no-build --nologo -o "%PACKAGE_OUT%" -p:Version="%PACKAGE_VERSION%" -p:AssemblyVersion="%LIBRARY_VERSION%" -p:FileVersion="%LIBRARY_VERSION%"
if errorlevel 1 goto fail

dotnet pack "%APICLIENT_TEST_PROJ%" -c Release --no-build --nologo -o "%PACKAGE_OUT%" -p:Version="%PACKAGE_VERSION%" -p:AssemblyVersion="%LIBRARY_VERSION%" -p:FileVersion="%LIBRARY_VERSION%"
if errorlevel 1 goto fail

set "APICLIENT_NUPKG="
set "APICLIENT_TEST_NUPKG="
for %%F in ("%PACKAGE_OUT%\*.nupkg") do call :assign_nupkg "%%~nxF"
if not defined APICLIENT_NUPKG (
    echo ERROR: Package file for MyLab.ApiClient was not found in "%PACKAGE_OUT%".
    exit /b 1
)
if not defined APICLIENT_TEST_NUPKG (
    echo ERROR: Package file for MyLab.ApiClient.Test was not found in "%PACKAGE_OUT%".
    exit /b 1
)

echo.
echo === Projects ===
call :show_project "MyLab.ApiClient" "%LIBRARY_VERSION%" "%LIBRARY_VERSION%" "%PACKAGE_VERSION%" "%APICLIENT_NUPKG%"
echo.
call :show_project "MyLab.ApiClient.Test" "%LIBRARY_VERSION%" "%LIBRARY_VERSION%" "%PACKAGE_VERSION%" "%APICLIENT_TEST_NUPKG%"
echo.

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

set /p "CONFIRM=Press Enter to publish: "
ver >nul

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

:usage
echo Usage: publish.bat ^<version^> [-suffix ^<value^>] [build-only]
echo   version     Required version in xx.xx.xx format, where xx is a number.
echo   -suffix     Optional non-empty package version suffix.
echo   build-only  Create packages without publishing.
exit /b 0

:validate_version
setlocal
set "V=%~1"
set "P1="
set "P2="
set "P3="
set "P4="
for /f "tokens=1-4 delims=." %%A in ("%V%") do (
    set "P1=%%A"
    set "P2=%%B"
    set "P3=%%C"
    set "P4=%%D"
)
if not defined P1 goto validate_fail
if not defined P2 goto validate_fail
if not defined P3 goto validate_fail
if defined P4 goto validate_fail
if not "%V%"=="%P1%.%P2%.%P3%" goto validate_fail
call :is_digits "%P1%"
if errorlevel 1 goto validate_fail
call :is_digits "%P2%"
if errorlevel 1 goto validate_fail
call :is_digits "%P3%"
if errorlevel 1 goto validate_fail
endlocal
exit /b 0

:validate_fail
endlocal
exit /b 1

:is_digits
setlocal
set "N=%~1"
if "%N%"=="" goto digits_fail
for /f "delims=0123456789" %%D in ("%N%") do goto digits_fail
endlocal
exit /b 0

:is_digits4
setlocal
set "N=%~1"
if "%N%"=="" goto digits_fail
if not "%N:~4%"=="" goto digits_fail
if "%N:~3,1%"=="" goto digits_fail
for /f "delims=0123456789" %%D in ("%N%") do goto digits_fail
endlocal
exit /b 0

:digits_fail
endlocal
exit /b 1

:assign_nupkg
set "NAME=%~1"
if /I "%NAME:~0,21%"=="MyLab.ApiClient.Test." (
    set "APICLIENT_TEST_NUPKG=%~1"
    exit /b 0
)
if /I "%NAME:~0,16%"=="MyLab.ApiClient." (
    set "APICLIENT_NUPKG=%~1"
    exit /b 0
)
exit /b 0

:show_project
echo %~1
echo   Library version: %~2
echo   File version: %~3
echo   Package version: %~4
echo   Package file: %~5
exit /b 0
