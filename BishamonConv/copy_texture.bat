@echo off

@setlocal

@set OUTPUT_PATH="Output\Resources\"

xcopy /D /S /R /Y /I /K database\texture "%OUTPUT_PATH%texture\"
xcopy /D /S /R /Y /I /K /EXCLUDE:excludelist database\model "%OUTPUT_PATH%model\"

@endlocal
