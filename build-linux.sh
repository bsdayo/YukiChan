dotnet publish --nologo -r linux-x64 --no-self-contained -o ./Build/Linux/ ./YukiChan/YukiChan.csproj

rm ./Build/**/*.pdb
rm ./Build/**/*.xml
