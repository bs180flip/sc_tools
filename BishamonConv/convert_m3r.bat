@echo off

@setlocal

@set BMSLN_CONV="converter\bmconv.exe"
@set OUTPUT_PATH="Output\Resources\m3r"
@set CACHE_PATH="Output\cache\model"

@if "%1" == "/clean" (
	
	@if exist "%OUTPUT_PATH%\m3r" (
		@del /Q "%OUTPUT_PATH%\*.m3r.bytes"
		@del /Q "%OUTPUT_PATH%\*.m3r.bytes.meta"
	)
	
) else @if "%1" == "/update" (
	
	REM dae/fbx -> m3r
		
	for /r %%i in (databasa\model\*.dae) do (
		%BMSLN_CONV% -I"%%i" -O"%OUTPUT_PATH%\%%~ni.m3r.bytes"
	)
	for /r %%i in (databasa\model\*.fbx) do (
		%BMSLN_CONV% -I"%%i" -O"%OUTPUT_PATH%\%%~ni.m3r.bytes"
	)
	
) else (
	
	REM dae/fbx -> m3r
	
	for /r %%i in (database\model\*.dae) do (
		call :DefinitionFileUpdateCheck "%CACHE_PATH%\%%~ni.dae" "%%i"
		if errorlevel 1 (
			echo convert %%~ni.dae
			%BMSLN_CONV% -I"%%i" -O"%OUTPUT_PATH%\%%~ni.m3r.bytes"
			if errorlevel 1 (
				rem empty
			) else (
				echo F | xcopy "%%i" "%CACHE_PATH%\%%~ni.dae" /D /I /R /Y /S > nul
			)
		)
	)
	for /r %%i in (database\model\*.fbx) do (
		call :DefinitionFileUpdateCheck "%CACHE_PATH%\%%~ni.fbx" "%%i"
		if errorlevel 1 (
			echo convert %%~ni.fbx
			%BMSLN_CONV% -I"%%i" -O"%OUTPUT_PATH%\%%~ni.m3r.bytes"
			if errorlevel 1 (
				rem empty
			) else (
				echo F | xcopy "%%i" "%CACHE_PATH%\%%~ni.fbx" /D /I /R /Y /S > nul
			)
		)
	)
	
)

@endlocal
@exit /B 0

:DefinitionFileUpdateCheck

@if not exist %1 @exit /B 1
@if not exist %2 @exit /B 0

@if "%~t1" == "%~t2" @exit /B 0
@if "%~t1" gtr "%~t2" @exit /B 0

@exit /B 1
