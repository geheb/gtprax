#!/bin/bash

echo "GT Prax Deployment"
read -p "Enter version: " version

[[ "$version" =~ ^(v[0-9]+\.[0-9]+\.[0-9]+)$ ]] || { echo "This is not a semver!"; exit $ERRCODE; }

[ -d "$version" ] && rm -R "$version/"

mkdir "$version" && cd "$_"

wget https://github.com/geheb/gtprax/releases/download/$version/release-linux-x64.tar.xz
tar xf release-linux-x64.tar.xz

read -r -p "Copy to target? [y/N] " response
if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]
then
    cp -R app/* /opt/gtprax/
	chmod +x /opt/gtprax/GtPrax.WebApp
else
    echo "TODO: cp -R app/* /opt/gtprax/"
fi

