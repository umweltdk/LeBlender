@ECHO OFF

ECHO Removing old nupkg
del /S *.nupkg

ECHO Packing the NuGet release files
..\.nuget\NuGet.exe Pack Umwelt.LeBlender.nuspec -Prop Configuration=Release

PAUSE
