# Build
dotnet publish -c Release -r linux-arm64 --no-self-contained -o ./build/linux-arm64/ ./src/YukiChan/YukiChan.csproj
dotnet publish -c Release -r linux-arm64 --no-self-contained -o ./build/linux-arm64/ ./src/YukiChan.Tools/YukiChan.Tools.csproj

# Cleanup
rm ./build/linux-arm64/*.xml
rm ./build/linux-arm64/*.pdb
