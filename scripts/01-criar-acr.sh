
set -e

source "$(dirname "$0")/00-variaveis.sh"

echo ""
echo ">>> [1/3] Criando o Resource Group..."
az group create \
  --name "$RG_NAME" \
  --location "$LOCATION" \
  --output table

echo ""
echo ">>> [2/3] Criando o Azure Container Registry (SKU Basic)..."

az acr create \
  --resource-group "$RG_NAME" \
  --name "$ACR_NAME" \
  --sku Basic \
  --location "$LOCATION" \
  --admin-enabled true \
  --output table

echo ""
echo ">>> [3/3] Confirmando o registry criado..."
az acr show \
  --name "$ACR_NAME" \
  --query "{Nome:name, Servidor:loginServer, SKU:sku.name, Regiao:location}" \
  --output table

echo ""
echo "OK. Registry disponivel em: $ACR_LOGIN_SERVER"