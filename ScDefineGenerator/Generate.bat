@echo off
cd /d %~dp0

rmdir /s /q output

:�{�ԗp
:set DOC_DIR=../ScMstSqlGenerator/mst/
:�f�o�b�O�p
set DOC_DIR=../../sc_data/tools/ScMstSqlGenerator/mst/

call "exe/ScDefineGenerator.exe" "%DOC_DIR%"
