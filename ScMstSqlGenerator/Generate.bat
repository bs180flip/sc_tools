@echo off
cd /d %~dp0

cd ../../02_ƒ}ƒXƒ^ŠÇ—

if exist tmp rmdir /s /q tmp > nul 2>&1

if exist logs rmdir /s /q logs > nul 2>&1

echo D | xcopy /Y /S mst tmp > nul 2>&1

set DOC_DIR=tmp/

for %%f in (%*) do echo %%~nxf>> tmp/dragList.txt

call "../tools\ScMstSqlGenerator/exe/ScMstSqlGenerator.exe" "%DOC_DIR%"

rmdir /s /q tmp

pause
