dotnet publish --nologo -r linux-x64 --no-self-contained -o ./Build/linux-amd64/ ./YukiChan/YukiChan.csproj
dotnet publish --nologo -r linux-x64 --no-self-contained -o ./Build/linux-amd64/ ./YukiChan.Tools/YukiChan.Tools.csproj

rm ./Build/**/*.pdb
rm ./Build/**/*.xml
