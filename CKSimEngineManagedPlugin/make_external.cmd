@echo off
REM This script is meant to be run as a prebuild task in the visual studio project file
set "rootDir=%cd%\..\.."
echo %rootDir%
cd "%rootDir%\CKSimEngineManagedPlugin\CKSimEngineManagedPlugin"
rmdir /S /Q Generated
mkdir Generated
cd "%rootDir%"
cd CKSimEnginePlugin\CKSimProtoSpecLib\CKSimProtoSpec
for %%f in (*.proto) do (
    echo %%~nf
	"%rootDir%\CKSimEnginePlugin\CKprotobuf\compile\Release\protoc.exe" %%~nf.proto --csharp_out="%rootDir%\CKSimEngineManagedPlugin\CKSimEngineManagedPlugin\Generated" --csharp_opt=file_extension=.g.cs
)
echo Generated!
