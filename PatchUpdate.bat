@echo off
SETLOCAL
echo %~1
Set /a z=%1+1
if %~2==1 curl --verbose --request PUT --data %~1 --Header "Content-Type: text/plain" http://aotuo:1q2w3e4r@localhost:8080/httpAuth/app/rest/projects/lms/parameters/cloud.patch
if %~2==1 curl --verbose --request PUT --data %z% --Header "Content-Type: text/plain" http://aotuo:1q2w3e4r@localhost:8080/httpAuth/app/rest/projects/lms/parameters/cloud.pre.patch
ENDLOCAL