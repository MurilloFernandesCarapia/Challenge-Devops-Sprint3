
export LOCATION="southafricanorth"
export RG_NAME="rg-petcare360-sprint3"

export ACR_NAME="acrpetcare360sprint3"
export ACR_LOGIN_SERVER="${ACR_NAME}.azurecr.io"


export ACI_NAME="aci-petcare360"

export DNS_LABEL="petcare360-sprint3"


export IMAGE_API="petcare360-api:v1"
export IMAGE_ORACLE="oracle-xe:21-slim"


ENV_FILE="$(dirname "${BASH_SOURCE[0]}")/../.env"

if [ ! -f "$ENV_FILE" ]; then
  echo "ERRO: arquivo .env nao encontrado na raiz do repositorio."
  echo "      Rode: cp .env.example .env  e preencha as senhas."
  return 1 2>/dev/null || exit 1
fi

set -a
source "$ENV_FILE"
set +a

echo "Variaveis carregadas."
echo "  Regiao          : $LOCATION"
echo "  Resource Group  : $RG_NAME"
echo "  ACR             : $ACR_LOGIN_SERVER"