cd ..\Valuator\
start dotnet build
cd ..\RankCalculator\
start dotnet build
cd ..\EventLogger\
start dotnet build

set DB_MAIN=localhost:6000
set DB_RU=localhost:6001
set DB_EU=localhost:6002
set DB_ASIA=localhost:6003

set DB_MAIN_PASS=main
set DB_RU_PASS=ru
set DB_EU_PASS=eu
set DB_ASIA_PASS=asia

set RABBITMQ_DEFAULT_USER=rabbituser
set RABBITMQ_DEFAULT_PASS=rabbitpass

cd ..
docker-compose up -d

pause
cd RankCalculator
start "RankCalculator" dotnet run --no-build
start "RankCalculator" dotnet run --no-build

cd ..\Valuator\
start "Valuator" dotnet run --no-build --urls "http://localhost:5001"
start "Valuator" dotnet run --no-build --urls "http://localhost:5002"


cd ..\EventLogger\
start "EventLogger" dotnet run --no-build
start "EventLogger" dotnet run --no-build

cd ..\nginx\
start nginx.exe