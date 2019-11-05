@echo off
setlocal

set MSBUILD_PATH=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

%MSBUILD_PATH% /p:Configuration=Release ScScenarioTools.sln
