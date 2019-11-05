@echo off

setlocal	enabledelayedexpansion

set BMSLN_CONV="..\converter\bmconv.exe"
set OUTPUT_PATH="..\Output\Resources"
set CACHE_PATH="..\Output\cache"

set BASE_PATH=%~dp0
set BASE_PATH=%BASE_PATH%database\

pushd database

if "%1" == "/clean" (

	pushd %OUTPUT_PATH%

	for /r %%i in (*.bmb.bytes) do (
		del /Q "%%i"
	)

	for /r %%i in (*.bmb.bytes.meta) do (
		del /Q "%%i"
	)

	popd
	
) else if "%1" == "/update" (
	
	REM bmsln -> bmb
		
	for /r %%i in (*.bmsln) do (
		set cache_file=%%i
		set cache_file=!cache_file:%BASE_PATH%=%CACHE_PATH%\!

		call :ConvertToBMB "%%i"

		if errorlevel 1 (
			rem empty
		) else (
			echo F | xcopy "%%i" "!cache_file!" /D /I /R /Y /S > nul
		)
	)
	
) else (
	
	REM bmsln -> bmb
	
	for /r %%i in (*.bmsln) do (
		set cache_file=%%i
		set cache_file=!cache_file:%BASE_PATH%=%CACHE_PATH%\!
		
		call :DefinitionFileUpdateCheck !cache_file!  "%%i"

		if errorlevel 1 (

			call :ConvertToBMB "%%i"

			if errorlevel 1 (
				rem empty
			) else (
				echo F | xcopy "%%i" "!cache_file!" /D /I /R /Y /S > nul
			)
		)
	)

)

popd
endlocal
exit /B 0


rem-------------------------------------------------------------------------------------------
:ConvertToBMB

	set src_path=%~dp1
	set src_path=!src_path:%BASE_PATH%=!
	set src_name=%~n1

	if "!src_path:~0,6!" == "bmsln\" (
		set dst_path=bmb\!src_path:~6!
		echo convert !src_name!.bmsln
	) else (
		set dst_path=!src_path!
		echo convert !src_path!!src_name!.bmsln
	)

	%BMSLN_CONV% -I%1 -O"%OUTPUT_PATH%\!dst_path!!src_name!.bmb.bytes" --database=%BASE_PATH%
	
	if errorlevel 1 (
		exit /B 1
	)

exit /B 0


rem-------------------------------------------------------------------------------------------
:DefinitionFileUpdateCheck

if not exist %1 @exit /B 1
if not exist %2 @exit /B 0

if "%~t1" == "%~t2" @exit /B 0
if "%~t1" gtr "%~t2" @exit /B 0

exit /B 1
