@echo off

if exist output rmdir /s /q output > nul 2>&1

if not exist output mkdir output
if not exist output\generate mkdir output\generate
if not exist output\document mkdir output\document
if not exist output\convert mkdir output\convert
if not exist output\parser mkdir output\parser

echo SourceGenerator -----------------------------------------------------------
call bin\Release\ScScenarioSourceGenerator.exe ^
  --arg_type_file test\ArgType.csv ^
  --command_dir test\command ^
  --output_dir output\generate\

echo.

echo DocumentConverter ---------------------------------------------------------
call bin\Release\ScScenarioDocumentConverter.exe ^
  --arg_type_file test\ArgType.csv ^
  --command_dir test\command ^
  --output_dir output\document\

echo.

echo Converter -----------------------------------------------------------------
call bin\Release\ScScenarioConverter.exe ^
  --book_file test\test.xlsx ^
  --arg_type_file test\ArgType.csv ^
  --command_dir test\command ^
  --global_define_file test\define.txt ^
  --local_define_dir test\define\ ^
  --macro_dir test\macro\ ^
  --output_csv_dir output\convert\ ^
  --output_bin_dir output\convert\

echo.

echo Parser --------------------------------------------------------------------
call bin\Release\ScScenarioParser.exe ^
  --bytes_file output\convert\test\home_enter.bytes ^
  --arg_type_file test\ArgType.csv ^
  --command_dir test\command > output\parser\parse.csv
