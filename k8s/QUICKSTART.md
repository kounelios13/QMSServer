# Quick Start Guide

This is a quick reference for deploying QMS Server to Kubernetes. For detailed information, see [README.md](README.md).

## Prerequisites Checklist

- [ ] Kubernetes cluster is running
- [ ] `kubectl` is installed and configured
- [ ] NGINX Ingress Controller is installed
- [ ] cert-manager is installed
- [ ] Docker image is built and pushed to registry
- [ ] DNS A record points to Ingress IP
- [ ] Passwords updated in `secret.yaml`

## 5-Minute Deployment

### 1. Update Configuration (REQUIRED!)

```bash
# Edit secret.yaml - CHANGE ALL PASSWORDS!
vim k8s/secret.yaml

# Edit deployment.yaml - Update Docker image
vim k8s/qms-deployment.yaml

# Edit clusterissuer.yaml - Update email
vim k8s/clusterissuer.yaml
```

### 2. Deploy Everything

```bash
# Option A: Use the deployment script (recommended)
./k8s/deploy.sh

# Option B: Use kubectl directly
kubectl apply -k k8s/
kubectl apply -f k8s/clusterissuer.yaml
```

### 3. Verify

```bash
# Check pods
kubectl get pods -n qms

# Check certificate (wait 1-2 minutes)
kubectl get certificate -n qms

# Check ingress
kubectl get ingress -n qms
```

### 4. Access

Open https://qms.mkcodergr.eu

## Quick Commands

```bash
# View logs
kubectl logs -f deployment/qms-app -n qms

# Scale application
kubectl scale deployment/qms-app -n qms --replicas=3

# Restart pods
kubectl rollout restart deployment/qms-app -n qms

# Delete everything
./k8s/cleanup.sh
```

## Common Issues

| Issue | Solution |
|-------|----------|
| Certificate not issuing | Wait 2-5 minutes, check DNS, verify cert-manager logs |
| 502 Bad Gateway | Check if pods are ready: `kubectl get pods -n qms` |
| Database connection error | Verify MySQL is running and passwords match in secret |
| Ingress not working | Verify NGINX Ingress Controller is installed |

## Important Notes

⚠️ **Change passwords in `secret.yaml` before production deployment!**

⚠️ **Test with staging issuer first**: Use `clusterissuer-staging.yaml`

✅ **WebSocket support enabled**: SignalR will work automatically

✅ **HTTPS only**: HTTP automatically redirects to HTTPS

## Need Help?

See the full [README.md](README.md) for:
- Detailed prerequisites
- Step-by-step instructions
- Troubleshooting guide
- Security best practices
