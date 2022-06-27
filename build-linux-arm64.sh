dotnet publish --nologo -r linux-arm64 --no-self-contained -o ./Build/linux-arm64/ ./YukiChan/YukiChan.csproj
dotnet publish --nologo -r linux-arm64 --no-self-contained -o ./Build/linux-arm64/ ./YukiChan.Tools/YukiChan.Tools.csproj

rm ./Build/**/*.pdb
rm ./Build/**/*.xml
