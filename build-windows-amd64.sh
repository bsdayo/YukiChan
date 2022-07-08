BUILD_DIR="windows-amd64"

dotnet publish --nologo -r win-x64 --no-self-contained -o ./Build/${BUILD_DIR}/ ./YukiChan/YukiChan.csproj
dotnet publish --nologo -r win-x64 --no-self-contained -o ./Build/${BUILD_DIR}/ ./YukiChan.Tools/YukiChan.Tools.csproj

rm ./Build/${BUILD_DIR}/*.pdb
rm ./Build/${BUILD_DIR}/*.xml
rm ./Build/${BUILD_DIR}/*.deps.json
rm ./Build/${BUILD_DIR}/*.runtimeconfig.json
