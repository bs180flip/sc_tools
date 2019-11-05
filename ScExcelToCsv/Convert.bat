@echo off
setlocal
cd /d %~dp0

set SRC_DIR=data/
set OUT_DIR=output/
set EXE=exe/ScExcelToCsv.exe

if "%~1" == "" goto main

:loop

if "%~1" == "" goto end

echo "%SRC_DIR%%~nx1"
:call "%EXE%" "%SRC_DIR%%~nx1" "%OUT_DIR%"

shift

goto loop

:main

for %%i in (%SRC_DIR%*.xlsx) do (
  call "%EXE%" "%SRC_DIR%%%i" "%OUT_DIR%"
)

:end

pause
