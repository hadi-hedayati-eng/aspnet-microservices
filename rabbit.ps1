docker pull mirror-docker.runflare.com/library/rabbitmq:management

docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 mirror-docker.runflare.com/library/rabbitmq:management