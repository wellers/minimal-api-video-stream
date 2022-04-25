    sudo docker build -t minimalapi .
    sudo docker run -d -p 80:80 --name minimalapi minimalapi