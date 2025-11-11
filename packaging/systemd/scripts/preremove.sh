#!/bin/bash
set -e

echo "Stopping DNSSEC-Manager before removal..."

if systemctl list-units --full -all | grep -q "dnssec-manager.service"; then
    systemctl stop dnssec-manager || true
    systemctl disable dnssec-manager || true
fi

exit 0
