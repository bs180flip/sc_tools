@echo off
cd /d %~dp0

rmdir /s /q output

:本番用
:set DOC_DIR=../ScMstSqlGenerator/mst/
:デバッグ用
set DOC_DIR=../../sc_data/tools/ScMstSqlGenerator/mst/

call "exe/ScDefineGenerator.exe" "%DOC_DIR%"
