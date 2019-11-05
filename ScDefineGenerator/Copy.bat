@echo off
cd /d %~dp0

:set COMMON_DST_DIR=..\..\..\sc_client\UnityProject\Assets\ScAssets\Common\Generated\
set COMMON_DST_DIR=..\..\sc_client\UnityProject\Assets\ScAssets\Common\Generated\

xcopy output\DefineMstConst.cs %COMMON_DST_DIR%Const\ /Y
xcopy output\InitialTextMaster.cs %COMMON_DST_DIR%MasterData\ /Y
xcopy output\InitialTextMstKey.cs %COMMON_DST_DIR%Const\ /Y
xcopy output\TextMaster.cs %COMMON_DST_DIR%MasterData\ /Y
xcopy output\TextMstKey.cs %COMMON_DST_DIR%Const\ /Y
