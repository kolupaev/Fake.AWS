@echo off
cls
cd %~dp0
".\packages\FAKE\tools\Fake.exe" "example_build.fsx" %1
