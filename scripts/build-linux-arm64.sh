# Build
dotnet publish -c Release -r linux-arm64 --no-self-contained -o ./build/linux-arm64/ ./YukiChan

# Cleanup
rm ./build/linux-arm64/*.xml
rm ./build/linux-arm64/*.pdb
