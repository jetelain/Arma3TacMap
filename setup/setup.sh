#!/bin/sh

# Basic script to install / update a GameTacMap instance on a Linux server
# Requires dotnet SDK, see https://learn.microsoft.com/en-us/dotnet/core/install/linux, `sudo apt-get install -y dotnet-sdk-8.0` on Ubuntu 24.04 LTS

if [ ! -d ~/build/GameTacMap ]; then
	mkdir ~/build
	cd ~/build
	git clone https://github.com/jetelain/Arma3TacMap.git GameTacMap
fi

if [ ! -d /opt/GameTacMap ]; then
	sudo mkdir /opt/GameTacMap
	sudo chown $USER:$USER /opt/GameTacMap
fi

if [ ! -d /var/www/GameTacMap ]; then
	sudo mkdir /var/www/GameTacMap
	sudo chown www-data:www-data /var/www/GameTacMap
	
	sudo mkdir /var/www/GameTacMap/storage
	sudo chown www-data:www-data /var/www/GameTacMap/storage
fi

if [ ! -d /var/www/aspnet-keys ]; then
	sudo mkdir /var/www/aspnet-keys
	sudo chown www-data:www-data /var/www/aspnet-keys
fi

cd ~/build/GameTacMap

echo "Update git"
git checkout main
git pull

echo "Check config"
if [ ! -f /opt/GameTacMap/appsettings.Production.json ]; then
	echo " * Create appsettings.Production.json"
	cp setup/appsettings.Production.json /opt/GameTacMap/appsettings.Production.json
	read -p "Type the Steam Api Key obtained from https://steamcommunity.com/dev/apikey, then press [ENTER]:" STEAM_API_KEY
	read -p "Type your Steam Id (for admin access), then press [ENTER]:" STEAM_ADMIN_ID
	sed -i "s/STEAM_API_KEY/$STEAM_API_KEY/g"  /opt/GameTacMap/appsettings.Production.json
	sed -i "s/STEAM_ADMIN_ID/$STEAM_ADMIN_ID/g"  /opt/GameTacMap/appsettings.Production.json
fi

if [ ! -f /etc/systemd/system/kestrel-tacmap.service ]; then
	echo " * Create kestrel-tacmap.service"
	sudo cp setup/kestrel-tacmap.service /etc/systemd/system/kestrel-tacmap.service
fi

echo "Build"
rm -rf dotnet-webapp
dotnet publish -c Release -o dotnet-webapp -r linux-x64 --self-contained false Arma3TacMapWebApp/Arma3TacMapWebApp.csproj

echo "Stop Service"
sudo systemctl stop kestrel-tacmap

echo "Copy files"
cp -ar "dotnet-webapp/." "/opt/GameTacMap"

echo "Start Service"
sudo systemctl start kestrel-tacmap
