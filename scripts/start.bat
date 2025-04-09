cd ..\Valuator\
start dotnet build
cd ..\RankCalculator\
start dotnet build
cd ..\EventLogger\
start dotnet build

cd ..\Valuator\
start dotnet run --no-build --urls "http://localhost:5001"
start dotnet run --no-build --urls "http://localhost:5002"

cd ..\RankCalculator\
docker-compose up -d
start dotnet run --no-build
start dotnet run --no-build

cd ..\EventLogger\
start dotnet run --no-build

cd ..\nginx\
start nginx.exe