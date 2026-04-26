# 1. Clean old Gateway API CRDs
kubectl get crd | Select-String "gateway.networking.k8s.io" | ForEach-Object { 
    $crd = ($_ -split '\s+')[0]
    kubectl delete crd $crd 
}

# 2. Remove safe-upgrade policy
kubectl delete validatingadmissionpolicy safe-upgrades.gateway.networking.k8s.io
kubectl delete validatingadmissionpolicybinding safe-upgrades.gateway.networking.k8s.io

# 3. Apply with server-side flag
kubectl apply --server-side -f "install.yaml"

kubectl describe pod envoy-default-my-gateway-1c7c06f0-56b964fc8-52tj6 -n envoy-gateway-system | Select-String "Image:"