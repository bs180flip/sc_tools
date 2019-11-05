@echo off
cd /d %~dp0

echo D | xcopy /Y /S mst tmp > nul 2>&1

set DOC_DIR=tmp/

call "exe/ScMstSqlGenerator.exe" "%DOC_DIR%" "MstCreate"

pause

rmdir /s /q tmp
