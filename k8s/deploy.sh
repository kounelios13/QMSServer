#!/bin/bash

# Kubernetes deployment script for QMS Server
# This script deploys the QMS application with Let's Encrypt certificates

set -e

echo "🚀 Starting QMS Server Kubernetes Deployment"
echo "=============================================="

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if kubectl is available
if ! command -v kubectl &> /dev/null; then
    echo "❌ kubectl is not installed. Please install kubectl first."
    exit 1
fi

echo -e "\n${YELLOW}Step 1: Creating namespace${NC}"
kubectl apply -f k8s/namespace.yaml

echo -e "\n${YELLOW}Step 2: Creating ClusterIssuer for Let's Encrypt${NC}"
kubectl apply -f k8s/clusterissuer.yaml

echo -e "\n${YELLOW}Step 3: Creating ConfigMap and Secrets${NC}"
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml

echo -e "\n${YELLOW}Step 4: Deploying MySQL Database${NC}"
kubectl apply -f k8s/mysql-pvc.yaml
kubectl apply -f k8s/mysql-statefulset.yaml
kubectl apply -f k8s/mysql-service.yaml

echo -e "\n${GREEN}Waiting for MySQL to be ready...${NC}"
kubectl wait --for=condition=ready pod -l app=mysql -n qms --timeout=300s || echo "Warning: MySQL may still be starting"

echo -e "\n${YELLOW}Step 5: Deploying QMS Application${NC}"
kubectl apply -f k8s/qms-deployment.yaml
kubectl apply -f k8s/qms-service.yaml

echo -e "\n${GREEN}Waiting for QMS application to be ready...${NC}"
kubectl wait --for=condition=available deployment/qms-app -n qms --timeout=300s || echo "Warning: QMS app may still be starting"

echo -e "\n${YELLOW}Step 6: Creating Ingress with TLS${NC}"
kubectl apply -f k8s/ingress.yaml

echo -e "\n${GREEN}=============================================="
echo "✅ Deployment Complete!"
echo "=============================================="
echo -e "${NC}"

echo "📊 Checking deployment status..."
echo ""
kubectl get all -n qms
echo ""

echo "🔒 Checking certificate status..."
echo ""
kubectl get certificate -n qms 2>/dev/null || echo "Certificate is being provisioned (this may take a few minutes)..."
echo ""

echo "🌐 Checking ingress status..."
echo ""
kubectl get ingress -n qms
echo ""

echo "📝 Next steps:"
echo "1. Ensure your DNS record points to the NGINX Ingress IP:"
echo "   kubectl get svc -n ingress-nginx"
echo ""
echo "2. Wait for the Let's Encrypt certificate to be issued (usually 1-2 minutes):"
echo "   kubectl get certificate -n qms -w"
echo ""
echo "3. Access your application at: https://qms.mkcodergr.eu"
echo ""
echo "4. Check logs if needed:"
echo "   kubectl logs -f deployment/qms-app -n qms"
echo ""
