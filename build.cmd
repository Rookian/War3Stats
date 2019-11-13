rmdir /Q /S .\build
dotnet publish .\WC3Stats.Server -o .\build -c Release

pushd .\WC3Stats.Client
cmd /c ng build --outputPath=.\..\build\wwwroot --prod=true
popd

powershell -Command "Compress-Archive -Path .\build\* -DestinationPath build.zip -Force" 
