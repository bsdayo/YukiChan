$TARGET = "win-x64"

# Build
dotnet publish -c Release -r $TARGET --no-self-contained -o "./build/$TARGET/" ./src/YukiChan/YukiChan.csproj
dotnet publish -c Release -r $TARGET --no-self-contained -o "./build/$TARGET/" ./src/YukiChan.Tools/YukiChan.Tools.csproj

# Cleanup
rm ./build/win-x64/*.xml
rm ./build/win-x64/*.pdb
