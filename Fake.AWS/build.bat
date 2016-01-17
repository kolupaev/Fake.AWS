@echo off
cls
cd %~dp0
".\packages\FAKE\tools\Fake.exe" "build.fsx" %1
