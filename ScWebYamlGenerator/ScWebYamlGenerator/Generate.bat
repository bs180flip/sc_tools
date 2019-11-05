@echo off
cd /d %~dp0

set DOC_DIR=../../../sc/tools/Sc-Documents-Tools/

set MST_DIR=../../../sc/tools/ScMstSqlGenerator/mst/

call "exe/ScWebYamlGenerator.exe" "%DOC_DIR%" "%MST_DIR%"
