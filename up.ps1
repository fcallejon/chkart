docker rm $(docker ps -aq)
.\build-docker.ps1
docker-compose up -d redis
sleep 3
docker-compose up -d webapi
sleep 2
# docker-compose up --scale webapi=3
# sleep 2
docker-compose up -d load_balancer