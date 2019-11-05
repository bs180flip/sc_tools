@echo off
cd /d %~dp0

set EXE_NAME=ScScenarioConverter.exe
set EXE_SRC_DIR=exe\
set EXE_DST_DIR=..\..\sc_data\tools\ScScenarioConverter\exe\

copy /Y %EXE_SRC_DIR%%EXE_NAME% %EXE_DST_DIR%%EXE_NAME%
