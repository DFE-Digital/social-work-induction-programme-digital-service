docker build \
    --build-arg AZURE_KUDU_SSH_USER=root \
    --secret id=AZURE_KUDU_SSH_PASSWORD,env=AZURE_KUDU_SSH_PASSWORD \
    --build-arg FULL_TAG=auth-service \
    -f "./Dockerfile" \
    --network=host \
    --target auth-service \
    -t "auth-service" .

docker run --rm -it \
  --entrypoint sh \
  auth-service