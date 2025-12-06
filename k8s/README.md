# Kubernetes Deployment for QMS Server

This directory contains Kubernetes YAML files for deploying the QMS Server application with Let's Encrypt TLS certificates.

## Prerequisites

Before deploying, ensure you have the following installed in your Kubernetes cluster:

1. **NGINX Ingress Controller**
   ```bash
   kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.2/deploy/static/provider/cloud/deploy.yaml
   ```

2. **cert-manager** (for Let's Encrypt certificates)
   ```bash
   kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.2/cert-manager.yaml
   ```

3. **Docker Image**: Build and push your QMS Docker image
   ```bash
   cd QMS
   docker build -t your-registry/qms-server:latest -f Dockerfile ..
   docker push your-registry/qms-server:latest
   ```

## Configuration

### ⚠️ CRITICAL: Update Secret Passwords (Required for Production)

**Before deploying to production**, you **MUST** update the passwords in `k8s/secret.yaml`:
- `MYSQL_ROOT_PASSWORD` - Replace with a strong, unique password (minimum 32 characters)
- `MYSQL_PASSWORD` - Replace with a strong, unique password (minimum 32 characters)
- `ConnectionStrings__DefaultConnection` - Update the password to match `MYSQL_PASSWORD`

**Security Best Practices:**
- Use a password manager to generate cryptographically secure passwords
- Never commit real passwords to version control
- Consider using Kubernetes secrets management solutions:
  - [Sealed Secrets](https://github.com/bitnami-labs/sealed-secrets) - Encrypt secrets in Git
  - [External Secrets Operator](https://external-secrets.io/) - Sync from external secret stores
  - [SOPS](https://github.com/mozilla/sops) - Encrypted files in Git

### Update Docker Image

In `k8s/qms-deployment.yaml`, update the image reference:
```yaml
image: your-registry/qms-server:latest
```

### Update Email in ClusterIssuer

In `k8s/clusterissuer.yaml`, update the email address for Let's Encrypt notifications:
```yaml
email: your-email@example.com
```

### Testing with Let's Encrypt Staging (Optional)

For testing purposes, you can use the Let's Encrypt staging environment which has higher rate limits and doesn't count against production limits. To use staging:

1. Deploy the staging ClusterIssuer:
   ```bash
   kubectl apply -f k8s/clusterissuer-staging.yaml
   ```

2. Update the ingress annotation in `k8s/ingress.yaml`:
   ```yaml
   cert-manager.io/cluster-issuer: letsencrypt-staging
   ```

⚠️ Note: Staging certificates will show a security warning in browsers. Switch to production after testing.

## Deployment Steps

### 1. Create the Namespace
```bash
kubectl apply -f k8s/namespace.yaml
```

### 2. Deploy cert-manager ClusterIssuer
```bash
kubectl apply -f k8s/clusterissuer.yaml
```

### 3. Create ConfigMap and Secrets
```bash
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
```

### 4. Deploy MySQL Database
```bash
kubectl apply -f k8s/mysql-pvc.yaml
kubectl apply -f k8s/mysql-statefulset.yaml
kubectl apply -f k8s/mysql-service.yaml
```

Wait for MySQL to be ready:
```bash
kubectl wait --for=condition=ready pod -l app=mysql -n qms --timeout=300s
```

### 5. Deploy QMS Application
```bash
kubectl apply -f k8s/qms-deployment.yaml
kubectl apply -f k8s/qms-service.yaml
```

### 6. Deploy Ingress with Let's Encrypt
```bash
kubectl apply -f k8s/ingress.yaml
```

### Deploy All at Once
Alternatively, you can deploy everything at once using kustomize:
```bash
kubectl apply -k k8s/
```

## Verify Deployment

### Check Pod Status
```bash
kubectl get pods -n qms
```

### Check Certificate Status
```bash
kubectl get certificate -n qms
kubectl describe certificate qms-tls-cert -n qms
```

### Check Ingress
```bash
kubectl get ingress -n qms
kubectl describe ingress qms-ingress -n qms
```

### View Logs
```bash
# QMS Application logs
kubectl logs -f deployment/qms-app -n qms

# MySQL logs
kubectl logs -f statefulset/mysql -n qms
```

## DNS Configuration

Point your domain to the NGINX Ingress Controller's external IP:
```bash
kubectl get svc -n ingress-nginx
```

Create an A record for `qms.mkcodergr.eu` pointing to the external IP.

## Accessing the Application

Once deployed and the certificate is issued (may take a few minutes), access the application at:
- **HTTPS**: https://qms.mkcodergr.eu
- **API Documentation**: https://qms.mkcodergr.eu/scalar/v1 (in development mode)

## Troubleshooting

### Certificate Not Issuing
```bash
# Check cert-manager logs
kubectl logs -n cert-manager deployment/cert-manager

# Check certificate request
kubectl get certificaterequest -n qms
kubectl describe certificaterequest -n qms

# Check challenge
kubectl get challenge -n qms
kubectl describe challenge -n qms
```

### Application Not Starting
```bash
# Check events
kubectl get events -n qms --sort-by='.lastTimestamp'

# Check pod details
kubectl describe pod -l app=qms-app -n qms
```

### Database Connection Issues
```bash
# Test MySQL connectivity from QMS pod
kubectl exec -it deployment/qms-app -n qms -- sh
# Inside the pod, try to connect to MySQL
```

## Scaling

Scale the QMS application:
```bash
kubectl scale deployment/qms-app -n qms --replicas=3
```

## Updates

Update the application:
```bash
# Update the image
kubectl set image deployment/qms-app qms-app=your-registry/qms-server:new-tag -n qms

# Or apply the updated deployment file
kubectl apply -f k8s/qms-deployment.yaml
```

## Cleanup

To remove all resources:
```bash
kubectl delete namespace qms
kubectl delete clusterissuer letsencrypt-prod
```

## Files Description

- `namespace.yaml` - Creates the qms namespace
- `configmap.yaml` - Application configuration
- `secret.yaml` - Sensitive data (passwords, connection strings)
- `mysql-pvc.yaml` - Persistent volume claim for MySQL data
- `mysql-statefulset.yaml` - MySQL database deployment
- `mysql-service.yaml` - MySQL service
- `qms-deployment.yaml` - QMS application deployment
- `qms-service.yaml` - QMS application service
- `clusterissuer.yaml` - Let's Encrypt production certificate issuer
- `clusterissuer-staging.yaml` - Let's Encrypt staging certificate issuer (for testing)
- `ingress.yaml` - Ingress with TLS configuration for qms.mkcodergr.eu
- `kustomization.yaml` - Kustomize configuration for easy deployment
- `deploy.sh` - Automated deployment script
- `cleanup.sh` - Safe cleanup script
- `.env.example` - Example environment configuration
