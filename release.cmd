@echo off
call az login
echo Logged in
call az webapp deployment source config-zip --resource-group WC3StatsServer --name WC3StatsServer --src build.zip
echo Ready