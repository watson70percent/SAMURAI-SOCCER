@echo off
setlocal EnableDelayedExpansion
chcp 65001

rd /s /q src\Assets
copy /Y Assembly-CSharp.csproj docFX\src\Assembly-CSharp.csproj
copy /Y Assembly-CSharp-Editor.csproj src\Assembly-CSharp-Editor.csproj
copy /Y SAMURAI-SOCCER.sln docFX\src\SAMURAI-SOCCER.sln
xcopy /S /E /Y Assets\Plugins docFX\src\Assets\Plugins\
xcopy /S /E /Y Assets\Project docFX\src\Assets\Project\

for /f "usebackq" %%a in (`dir docFX\src\Assets\*.cs /b /s`) do (
    set fileName=%%a
    set addletter1=s
    set tmpName1=!fileName!!addletter1!
    powershell -NoProfile -ExecutionPolicy Unrestricted -Command "& { $MyPath = '%%a';$MyFile = Get-Content $MyPath; $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding($False);[System.IO.File]::WriteAllLines($MyPath, $MyFile, $Utf8NoBomEncoding)}"
    type docFX\begin.txt !fileName! > !tmpName1!
    type !tmpName1! docFX\end.txt > !fileName!
    del /Q !tmpName1!
)

cd docFX
docfx docfx.json