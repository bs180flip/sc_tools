@echo off
setlocal
cd /d %~dp0

set EXE=exe/ScScenarioConverter.exe
set DOC_NAME1=detail.html
set DOC_NAME2=menu.html
set DOC_DST_DIR=..\..\sc_data\05_�X�N���v�g\document\src\

call "%EXE%" "/D"

copy /Y "%DOC_NAME1%" "%DOC_DST_DIR%%DOC_NAME1%"
copy /Y "%DOC_NAME2%" "%DOC_DST_DIR%%DOC_NAME2%"

