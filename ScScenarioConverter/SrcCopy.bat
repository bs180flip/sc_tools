@echo off
cd /d %~dp0

set CLIENT_SRC_DIR=..\..\sc_client\UnityProject\Assets\ScAssets\Scripts\Scenario\
set TOOLS_DST_DIR=src\

copy /Y %CLIENT_SRC_DIR%ArgType.cs %TOOLS_DST_DIR%ArgType.cs
copy /Y %CLIENT_SRC_DIR%ArgInfo.cs %TOOLS_DST_DIR%ArgInfo.cs
copy /Y %CLIENT_SRC_DIR%CommandType.cs %TOOLS_DST_DIR%CommandType.cs
copy /Y %CLIENT_SRC_DIR%CommandInfo.cs %TOOLS_DST_DIR%CommandInfo.cs
copy /Y %CLIENT_SRC_DIR%CommandInfoDict.cs %TOOLS_DST_DIR%CommandInfoDict.cs
