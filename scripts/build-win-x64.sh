# Build
dotnet publish -c Release -r win-x64 --no-self-contained -o ./build/win-x64/ ./src/YukiChan/YukiChan.csproj

# Cleanup
rm ./build/win-x64/*.xml
rm ./build/win-x64/*.pdb
