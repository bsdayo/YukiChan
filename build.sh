dotnet publish --nologo -r win-x64 --no-self-contained -o ./Build/Windows/ ./YukiChan/YukiChan.csproj
dotnet publish --nologo -r linux-x64 --self-contained -o ./Build/Linux/ ./YukiChan/YukiChan.csproj

rm ./Build/**/*.pdb
rm ./Build/**/Konata*
