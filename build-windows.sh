dotnet publish --nologo -r win-x64 --no-self-contained -o ./Build/Windows/ ./YukiChan/YukiChan.csproj
dotnet publish --nologo -r win-x64 --no-self-contained -o ./Build/Windows/ ./YukiChan.Tools/YukiChan.Tools.csproj

rm ./Build/**/*.pdb
rm ./Build/**/*.xml
