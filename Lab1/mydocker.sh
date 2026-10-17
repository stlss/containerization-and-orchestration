#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
API_DIR="$SCRIPT_DIR/../LabSolution/LabApi"
CG=/sys/fs/cgroup/lab-api

# Сборка выполняется до применения лимитов к процессу запуска.
dotnet build "$API_DIR/LabApi.csproj" --configuration Debug

# Включаем контроллеры у родителя и создаём группу для API.
echo '+cpu +memory +pids' | sudo tee /sys/fs/cgroup/cgroup.subtree_control >/dev/null
sudo mkdir -p "$CG"

echo 268435456 | sudo tee "$CG/memory.max" >/dev/null   # 256 МиБ памяти
echo 0 | sudo tee "$CG/memory.swap.max" >/dev/null      # Без swap
echo '50000 100000' | sudo tee "$CG/cpu.max" >/dev/null # Половина ядра
echo 64 | sudo tee "$CG/pids.max" >/dev/null            # 64 процессы и потоки

# Переносим текущий Bash в группу.
echo "$$" | sudo tee "$CG/cgroup.procs" >/dev/null

cd "$API_DIR"

# Создаём namespaces.
exec unshare \
    --pid --fork \
    --mount --mount-proc \
    --net \
    --uts \
    --ipc \
    --user --map-root-user \
    bash -eu -c '
        hostname lab-container
        ip link set lo up

        # Сбрасываем capabilities и запрещаем повышение привилегий при запуске других процессов.
        exec setpriv \
            --bounding-set=-all \
            --inh-caps=-all \
            --ambient-caps=-all \
            --no-new-privs \
            dotnet bin/Debug/net10.0/LabApi.dll --urls http://localhost:5232
    '
