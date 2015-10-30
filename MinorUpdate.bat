@echo off
SETLOCAL
echo %~1
Set /a z=%1+1
curl --verbose --request PUT --data %z% --Header "Content-Type: text/plain" http://aotuo:1q2w3e4r@localhost:8080/httpAuth/app/rest/projects/lms/parameters/cloud.minor
ENDLOCAL