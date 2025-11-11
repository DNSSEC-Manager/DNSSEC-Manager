#!/bin/bash
set -e

SERVICE="dnssec-manager.service"
CONF_DIR="/etc/dnssec-manager"
CONF_FILE="$CONF_DIR/dnssec-manager.conf"

echo "Installing DNSSEC-Manager systemd service..."

# Create directory
if [ ! -d "$CONF_DIR" ]; then
    mkdir -p "$CONF_DIR"
    chmod 755 "$CONF_DIR"
fi

# Create default config file
if [ ! -f "$CONF_FILE" ]; then
    cat <<EOF > "$CONF_FILE"
# DNSSEC-Manager configuration
# Example:
# ASPNETCORE_ENVIRONMENT: Production
# HTTP_PORTS: 8080
EOF
    chmod 600 "$CONF_FILE"
    echo "Created default configuration file at $CONF_FILE"
fi

# Install systemd service
if [ -f "/lib/systemd/system/$SERVICE" ] || [ -f "/usr/lib/systemd/system/$SERVICE" ]; then
    systemctl daemon-reload
    systemctl enable dnssec-manager
    systemctl restart dnssec-manager || true
else
    echo "WARNING: Service file not found — systemd service not installed."
fi

echo "DNSSEC-Manager installation complete."
exit 0
