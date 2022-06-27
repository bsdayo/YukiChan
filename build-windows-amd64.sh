dotnet publish --nologo -r win-x64 --no-self-contained -o ./Build/windows-amd64/ ./YukiChan/YukiChan.csproj
dotnet publish --nologo -r win-x64 --no-self-contained -o ./Build/windows-amd64/ ./YukiChan.Tools/YukiChan.Tools.csproj

rm ./Build/**/*.pdb
rm ./Build/**/*.xml
