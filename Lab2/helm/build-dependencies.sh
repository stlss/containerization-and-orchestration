#!/usr/bin/env bash
set -euo pipefail

chart_root="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo add grafana-community https://grafana-community.github.io/helm-charts
helm repo add grafana https://grafana.github.io/helm-charts
helm repo add jaegertracing https://jaegertracing.github.io/helm-charts
helm repo update

for component in prometheus alertmanager grafana loki alloy jaeger; do
  helm dependency build "$chart_root/$component" --skip-refresh
done

helm dependency build "$chart_root/monitoring" --skip-refresh
