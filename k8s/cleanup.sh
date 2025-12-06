#!/bin/bash

# Kubernetes cleanup script for QMS Server
# This script removes all QMS resources from the cluster

set -e

echo "🗑️  QMS Server Kubernetes Cleanup"
echo "=================================="

# Colors
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "\n${YELLOW}⚠️  WARNING: This will delete all QMS resources including data!${NC}"
read -p "Are you sure you want to continue? (yes/no): " -r
echo

if [[ ! $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
    echo "Cleanup cancelled."
    exit 0
fi

echo -e "\n${RED}Deleting Ingress...${NC}"
kubectl delete -f k8s/ingress.yaml --ignore-not-found=true

echo -e "\n${RED}Deleting QMS Application...${NC}"
kubectl delete -f k8s/qms-service.yaml --ignore-not-found=true
kubectl delete -f k8s/qms-deployment.yaml --ignore-not-found=true

echo -e "\n${RED}Deleting MySQL Database...${NC}"
kubectl delete -f k8s/mysql-service.yaml --ignore-not-found=true
kubectl delete -f k8s/mysql-statefulset.yaml --ignore-not-found=true
kubectl delete -f k8s/mysql-pvc.yaml --ignore-not-found=true

echo -e "\n${RED}Deleting ConfigMap and Secrets...${NC}"
kubectl delete -f k8s/configmap.yaml --ignore-not-found=true
kubectl delete -f k8s/secret.yaml --ignore-not-found=true

echo -e "\n${RED}Deleting Namespace...${NC}"
kubectl delete -f k8s/namespace.yaml --ignore-not-found=true

read -p "Do you want to delete the ClusterIssuer? (yes/no): " -r
echo
if [[ $REPLY =~ ^[Yy][Ee][Ss]$ ]]; then
    echo -e "\n${RED}Deleting ClusterIssuer...${NC}"
    kubectl delete -f k8s/clusterissuer.yaml --ignore-not-found=true
fi

echo -e "\n✅ Cleanup complete!"
