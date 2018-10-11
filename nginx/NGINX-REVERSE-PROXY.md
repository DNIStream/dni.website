[Home](../README.md)

# NGINX Docker Host Reverse Proxy for Docker Containers

## Instructions

#### Let's Encrypt Installation

https://certbot.eff.org/lets-encrypt/ubuntubionic-nginx

##### Install Let's Encrypt / Certbot
```
sudo apt-get update && \
sudo apt-get install software-properties-common && \
sudo add-apt-repository ppa:certbot/certbot && \
sudo apt-get update && \
sudo apt-get install python-certbot-nginx
```

#### NGINX Installation & Configuration

Install NGINX:

```
sudo apt-get update && \
sudo apt-get install nginx
```

Generate SSL Certificate:

```
sudo certbot --nginx -d www.dnistream.live -d dnistream.live -d api.dnistream.live certonly
```

Once the SSL cert is generated and authed, disable the default NGINX website

```
sudo rm /etc/nginx/sites-enabled/default
```

##### Make server blocks available to NGINX and enable them

Only required the first time you set each Server Block up, or if you want to re-enable a previously disabled Server Block.

N.B. The SSL certificates must already exist on the server before any sites will be served.

```
sudo ln -s /etc/nginx/sites-available/http-to-https.conf /etc/nginx/sites-enabled \
&& sudo ln -s /etc/nginx/sites-available/prod-api.dnistream.live.conf /etc/nginx/sites-enabled \
&& sudo ln -s /etc/nginx/sites-available/prod-www.dnistream.live.conf /etc/nginx/sites-enabled
```

##### Restart NGINX

Restart required whenever a symlink is added / removed. Reload is required if existing configs are changed.

```
sudo service nginx restart
```
or
```
sudo service nginx reload
```
