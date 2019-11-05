@echo off
setlocal
cd /d %~dp0

set SRC_DIR=data/
set OUT_DIR=output/
set EXE=exe/ScScenarioConverter.exe

if "%~1" == "" goto main

:loop

if "%~1" == "" goto end

if exist "%~1\" (
  for /r "%~1\" %%i in (*.csv) do (
    call "%EXE%" "%%i" "%OUT_DIR%"
  )
) else if exist "%~1" (
  call "%EXE%" "%SRC_DIR%%~nx1" "%OUT_DIR%"
)

shift

goto loop

:main

for /r %SRC_DIR% %%i in (*.csv) do (
  call "%EXE%" "%%i" "%OUT_DIR%"
)

:end

pause
