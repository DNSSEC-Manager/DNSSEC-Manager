# DNSSEC Manager

DNSSEC Manager for PowerDNS

## Installation

1. Clone this git repository to your IDE of choice
2. Publish the project to your server with .NET core support
4. Browse to web application

## Prepare PowerDNS

To use this software you need a PowerDNS Authoritative Nameserver: https://doc.powerdns.com/authoritative/

Configure API access in /etc/pdns/pdns.conf and open your firewall\
Configuring SSL for PowerDNS api: https://www.paulhermans.eu/configuring-ssl-for-powerdns-api/

## First time configuration

1. Login to application (see default login below)
2. Click on "Hello admin!" and change the password and setup Two-factor authentication
3. Configure application:\
	a. Add DNS server (connect to PowerDNS API)\
	b. Add your nameserver-group(s) corresponding with your DNS server\
	c. Add your domain registries (connect to registry API)\
	d. Add TLD's corresponding with your registry (if needed)\
4. Run the scheduler for the first time yourapphostname.tld/Scheduler

NOTE: if you cannot connect to your Registry you might need to whitelist the webserver to access the API

## Configure Task Scheduler or Cronjob

Configure a Task Scheduler or Cronjob to run the Scheduler every hour: yourapphostname.tld/Scheduler

## Default login

Default login:\
Username: admin\
Password: ChangeMe123!\
Email: example@dnssecmanager.net

## Credits

Written by:
- Paul Hermans
- Dylan Bos (Internship 2019)

## License

This project is released under the MIT license. For additional information, [see the full license](LICENSE).