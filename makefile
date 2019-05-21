all:
	@echo
	@echo "\033[4m\033[1mBuilding NICE\033[0m"
	@echo
	@dotnet build src/NetEmu/NetEmu.csproj

run:
	@echo
	@echo "\033[4m\033[1mRunning NICE\033[0m"
	@echo
	@dotnet run --project src/NetEmu/NetEmu.csproj