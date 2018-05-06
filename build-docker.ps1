$exclude = @('obj','bin', '*.dll')
Copy-Item .\chktr-model .\api\chktr-model -Recurse -Exclude $exclude -force
docker build --rm -f .\api\Dockerfile -t chktr:latest .\api
Remove-Item .\api\chktr-model -Recurse -force