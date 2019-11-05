@echo off
cd /d %~dp0

set DST_DIR=..\..\..\Sc-Documents-Tools\source\

xcopy output %DST_DIR%yaml\ /Y


