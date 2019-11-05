@echo off

if exist output rmdir /s /q output > nul 2>&1
if not exist output mkdir output

call bin\Release\ScMakeCpk.exe ^
  --input_dir test\input\ ^
  --output_dir output\ ^
  --cpkmakec_path test\cri\cpkmakec.exe ^
  --config_xml_path test\config.xml

pause
