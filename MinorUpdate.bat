@echo off
SETLOCAL
echo %~1
Set /a z=%1+1
if %~2==1 curl --verbose --request PUT --data %~1 --Header "Content-Type: text/plain" http://%~3@172.16.7.4:81/httpAuth/app/rest/projects/%~4/parameters/cloud.minor
if %~2==1 curl --verbose --request PUT --data %z% --Header "Content-Type: text/plain" http://%~3@172.16.7.4:81/httpAuth/app/rest/projects/%~4/parameters/cloud.beta.minor
ENDLOCAL