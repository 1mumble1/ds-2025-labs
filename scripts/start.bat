cd ..\Valuator\
start dotnet build
cd ..\RankCalculator\
start dotnet build
cd ..\EventLogger\
start dotnet build

pause

cd ..\Valuator\
start "Valuator" dotnet run --no-build --urls "http://localhost:5001"
start "Valuator" dotnet run --no-build --urls "http://localhost:5002"

cd ..\RankCalculator\
docker-compose up -d
start "RankCalculator" dotnet run --no-build
start "RankCalculator" dotnet run --no-build

cd ..\EventLogger\
start "EventLogger" dotnet run --no-build
start "EventLogger" dotnet run --no-build

cd ..\nginx\
start nginx.exe