@echo off

cd /d %~dp0

set EXE_DST_DIR=..\..\sc_data\tools\ScScenarioTools\exe\

copy /Y bin\Release\*.exe %EXE_DST_DIR%
copy /Y bin\Release\*.dll %EXE_DST_DIR%
