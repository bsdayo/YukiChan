# Build
dotnet publish -c Release -r win-x64 --no-self-contained -o ./build/windows-amd64/ ./YukiChan

# Cleanup
rm ./build/windows-amd64/*.xml
rm ./build/windows-amd64/*.pdb
