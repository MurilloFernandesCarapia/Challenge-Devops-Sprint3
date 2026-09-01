
set -e

source "$(dirname "$0")/00-variaveis.sh"
SCRIPT_DIR="$(dirname "$0")"

TEMPLATE="$SCRIPT_DIR/aci-petcare360.template.yaml"
GERADO="$SCRIPT_DIR/aci-petcare360.yaml"

echo ""
echo ">>> [1/4] Lendo as credenciais do ACR..."
ACR_USERNAME=$(az acr credential show --name "$ACR_NAME" --query "username" -o tsv)
ACR_PASSWORD=$(az acr credential show --name "$ACR_NAME" --query "passwords[0].value" -o tsv)

echo ""
echo ">>> [2/4] Gerando o YAML a partir do template..."
sed \
  -e "s|__LOCATION__|$LOCATION|g" \
  -e "s|__ACI_NAME__|$ACI_NAME|g" \
  -e "s|__DNS_LABEL__|$DNS_LABEL|g" \
  -e "s|__ACR_LOGIN_SERVER__|$ACR_LOGIN_SERVER|g" \
  -e "s|__ACR_USERNAME__|$ACR_USERNAME|g" \
  -e "s|__ACR_PASSWORD__|$ACR_PASSWORD|g" \
  -e "s|__IMAGE_API__|$IMAGE_API|g" \
  -e "s|__IMAGE_ORACLE__|$IMAGE_ORACLE|g" \
  -e "s|__ORACLE_ADMIN_PASSWORD__|$ORACLE_ADMIN_PASSWORD|g" \
  -e "s|__APP_USER_PASSWORD__|$APP_USER_PASSWORD|g" \
  "$TEMPLATE" > "$GERADO"

echo ""
echo ">>> [3/4] Criando o container group no ACI..."
az container create \
  --resource-group "$RG_NAME" \
  --file "$GERADO"

echo ""
echo ">>> [4/4] Endereco publico da solucao:"
FQDN=$(az container show --resource-group "$RG_NAME" --name "$ACI_NAME" \
  --query "ipAddress.fqdn" -o tsv)
IP=$(az container show --resource-group "$RG_NAME" --name "$ACI_NAME" \
  --query "ipAddress.ip" -o tsv)

echo ""
echo "  Swagger : http://$FQDN:8080/swagger"
echo "  IP      : $IP"
echo "  Oracle  : $FQDN:1521/XEPDB1"
echo ""
echo "O Oracle leva de 1 a 3 minutos para inicializar. Acompanhe com:"
echo "  az container logs --resource-group $RG_NAME --name $ACI_NAME --container-name petcare-api --follow"