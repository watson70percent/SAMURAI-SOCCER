@echo off
setlocal EnableDelayedExpansion
chcp 65001

rd /s /q src\Assets
copy /Y ..\Assembly-CSharp.csproj src\Assembly-CSharp.csproj
copy /Y ..\Assembly-CSharp-Editor.csproj src\Assembly-CSharp-Editor.csproj
copy /Y ..\SAMURAI-SOCCER.sln src\SAMURAI-SOCCER.sln
xcopy /e ..\Assets\Plugins src\Assets\Plugins\
xcopy /e ..\Assets\Project src\Assets\Project\

for /f "usebackq" %%a in (`dir src\Assets\*.cs /b /s`) do (
    set fileName=%%a
    set addletter1=s
    set tmpName1=!fileName!!addletter1!
    powershell -NoProfile -ExecutionPolicy Unrestricted -Command "& { $MyPath = '%%a';$MyFile = Get-Content $MyPath; $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding($False);[System.IO.File]::WriteAllLines($MyPath, $MyFile, $Utf8NoBomEncoding)}"
    type begin.txt !fileName! > !tmpName1!
    type !tmpName1! end.txt > !fileName!
    del /Q !tmpName1!
)

docfx docfx\docfx.json