@echo off
cd /d %~dp0

set EXE_NAME=ScExcelToJsonCell.exe
set EXE_SRC_DIR=exe\
set EXE_DST_DIR=..\..\sc_data\tools\ScExcelToJsonCell\exe\

copy /Y %EXE_SRC_DIR%%EXE_NAME% %EXE_DST_DIR%%EXE_NAME%
