setup:
  @cd tools/onelogin-simulator && cd certs && mkcert -install && mkcert localhost

start:
  @cd tools/onelogin-simulator && docker-compose up -d

stop:
  @cd tools/onelogin-simulator && docker-compose down
