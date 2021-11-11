# backend-code-challenge
Challenge for lightfeather.io


Installation
1. Download the project
2. Bring up a terminal/shell in `backend-code-challenge/backend-code-challenge` (source files)
3. Run command `docker build -f backend-code-challenge/Dockerfile -t challenge .`
4. Once the project is built with Docker, run this command `docker run -d -p 1234:80 --name apis challenge`
5. The project should be running in the container. Go to this address `http://localhost:1234/` in your browser to see if it is working.

### Notes
* The project is a route-to-code which use only the ASP.net core and no web api. This means it is missing functionality such as data validation in models.
* My first time using Docker
