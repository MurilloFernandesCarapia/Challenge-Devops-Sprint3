
set -e

source "$(dirname "$0")/00-variaveis.sh"
REPO_ROOT="$(dirname "$0")/.."

echo ""
echo ">>> [1/4] Autenticando o Docker no ACR..."
az acr login --name "$ACR_NAME"

echo ""
echo ">>> [2/4] Buildando a imagem da API a partir do Dockerfile..."
docker build -t "$ACR_LOGIN_SERVER/$IMAGE_API" "$REPO_ROOT"

echo ""
echo ">>> [3/4] Enviando a imagem da API para o ACR..."
docker push "$ACR_LOGIN_SERVER/$IMAGE_API"

echo ""
echo ">>> [4/4] Importando a imagem do Oracle XE para o ACR..."

az acr import \
  --name "$ACR_NAME" \
  --source "docker.io/gvenzl/oracle-xe:21-slim" \
  --image "$IMAGE_ORACLE" \
  --force

echo ""
echo ">>> Imagens disponiveis no registry:"
az acr repository list --name "$ACR_NAME" --output table

echo ""
echo "OK. As duas imagens estao no ACR."