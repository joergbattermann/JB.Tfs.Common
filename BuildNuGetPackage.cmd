@echo off
if not exist .\Binaries mkdir .\Binaries
if not exist .\Binaries\NuGet mkdir .\Binaries\NuGet
.\Utils\nuget.exe pack -sym JB.Tfs.Common.nuspec -o .\Binaries\NuGet
pause