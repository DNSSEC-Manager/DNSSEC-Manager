# DNSSEC Manager for PowerDNS

**DNSSEC Manager** is a tool for DNS administrators that connects your PowerDNS nameservers with your domain registrars. It automates the signing of DNS zones with DNSSEC and uploads the signing keys to the domain registrars.

---

## Quick Start (Docker)

Get the DNSSEC Manager running in just a few steps:

```bash
# 1. Clone the repository
git clone https://github.com/yourusername/DNSSEC-Manager.git
cd DNSSEC-Manager

# 2. Start the Docker containers
docker compose up -d

# 3. Open the web interfaces
# PowerDNS statistics
http://localhost:8081/
# DNSSEC Manager
http://localhost:5000/
```

**Default login:**
- Username: `admin`
- Password: `ChangeMe123!`

---

## What Docker Compose Spins Up 🐳

When you run `docker compose up -d`, three containers are started automatically:

| Container | Purpose                                     | Emoji |
|-----------|---------------------------------------------|-------|
| `backend` | The **DNSSEC Manager** web application      | 🖥️   |
| `pdns`    | **PowerDNS Authoritative Nameserver**       | 🌐   |
| `db`      | **MariaDB for PowerDNS database storage** | 💾  |

This setup ensures your DNSSEC Manager can communicate with PowerDNS and store data automatically, without any extra configuration.

---

## How it works

The application connects to your **PowerDNS Authoritative Nameserver** via its API and to your **domain registrars** via their API. It checks whether a domain can be signed with DNSSEC to provide enhanced security, and handles the signing and key uploads automatically.

---

## Installation on a server

1. Clone the repository to your IDE or server.
2. Publish the project with **.NET** support.
3. Create folder /storage and make writeable
4. Browse to the web application URL to start using it.

---

## Prepare PowerDNS

To use this software, you need a **PowerDNS Authoritative Nameserver**: [PowerDNS Authoritative Guide](https://doc.powerdns.com/authoritative/)

- Configure API access in `/etc/pdns/pdns.conf`
- Ensure your firewall allows access
- For SSL configuration: [Configuring SSL for PowerDNS API](https://www.paulhermans.eu/configuring-ssl-for-powerdns-api/)

---

## First-time setup

1. Log in to the application (see default login above).
2. Click **"Hello admin!"** to change the password and enable **Two-Factor Authentication**.
3. Configure the application:
    - Add your **DNS server** (connect to PowerDNS API)
    - Add **Nameserver Groups** corresponding to your DNS server
    - Add your **Domain Registries** (connect to registry API)
    - Add **TLDs** corresponding with your registry (if needed)
4. Run the scheduler for the first time:
   ```
   yourapphostname.tld/Scheduler
   ```

> **Note:** If you cannot connect to your registry, you may need to whitelist your web server to access the API.

---

## Scheduler / Cronjob

Configure a **Task Scheduler** (Windows) or **Cronjob** (Linux) to run the scheduler every hour:

```
yourapphostname.tld/Scheduler
```

---

## Default login

- **Username:** admin
- **Password:** ChangeMe123!
- **Email:** example@dnssecmanager.net

> Change the default credentials immediately after first login.

---

## Credits

- Paul Hermans
- Dylan Bos (Internship 2019)

---

## License

This project is released under the **MIT License**. For details, see the [LICENSE](LICENSE) file.
