@echo off
set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
    -d json ^
    -c cs-simple-json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=../Assets/Resources/_Data ^
    -x outputCodeDir=../Assets/__Scripts/__ProjectBase/_Data/GenDataCode

pause