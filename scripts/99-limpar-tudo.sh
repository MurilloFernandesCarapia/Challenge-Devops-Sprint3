
set -e

source "$(dirname "$0")/00-variaveis.sh"

echo ""
echo "ATENCAO: isso apaga o Resource Group '$RG_NAME' e tudo dentro dele:"
echo "  - Azure Container Registry ($ACR_NAME) e as imagens"
echo "  - Container group do ACI ($ACI_NAME)"
echo "  - IP publico e DNS"
echo ""
read -p "Digite CONFIRMAR para prosseguir: " RESPOSTA

if [ "$RESPOSTA" != "CONFIRMAR" ]; then
  echo "Cancelado. Nada foi removido."
  exit 0
fi

echo ""
echo ">>> Removendo o Resource Group..."
az group delete --name "$RG_NAME" --yes

echo ""
echo ">>> Confirmando a remocao..."
az group show --name "$RG_NAME" --output table 2>&1 || true

echo ""
echo "OK. Recursos removidos."