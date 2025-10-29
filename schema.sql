CREATE TABLE domains (
  id                    INT AUTO_INCREMENT,
  name                  VARCHAR(255) NOT NULL,
  master                VARCHAR(128) DEFAULT NULL,
  last_check            INT DEFAULT NULL,
  type                  VARCHAR(8) NOT NULL,
  notified_serial       INT UNSIGNED DEFAULT NULL,
  account               VARCHAR(40) CHARACTER SET 'utf8' DEFAULT NULL,
  options               VARCHAR(64000) DEFAULT NULL,
  catalog               VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (id)
) Engine=InnoDB CHARACTER SET 'latin1';

CREATE UNIQUE INDEX name_index ON domains(name);
CREATE INDEX catalog_idx ON domains(catalog);


CREATE TABLE records (
  id                    BIGINT AUTO_INCREMENT,
  domain_id             INT DEFAULT NULL,
  name                  VARCHAR(255) DEFAULT NULL,
  type                  VARCHAR(10) DEFAULT NULL,
  content               VARCHAR(64000) DEFAULT NULL,
  ttl                   INT DEFAULT NULL,
  prio                  INT DEFAULT NULL,
  disabled              TINYINT(1) DEFAULT 0,
  ordername             VARCHAR(255) BINARY DEFAULT NULL,
  auth                  TINYINT(1) DEFAULT 1,
  PRIMARY KEY (id)
) Engine=InnoDB CHARACTER SET 'latin1';

CREATE INDEX nametype_index ON records(name,type);
CREATE INDEX domain_id ON records(domain_id);
CREATE INDEX ordername ON records (ordername);


CREATE TABLE supermasters (
  ip                    VARCHAR(64) NOT NULL,
  nameserver            VARCHAR(255) NOT NULL,
  account               VARCHAR(40) CHARACTER SET 'utf8' NOT NULL,
  PRIMARY KEY (ip, nameserver)
) Engine=InnoDB CHARACTER SET 'latin1';


CREATE TABLE comments (
  id                    INT AUTO_INCREMENT,
  domain_id             INT NOT NULL,
  name                  VARCHAR(255) NOT NULL,
  type                  VARCHAR(10) NOT NULL,
  modified_at           INT NOT NULL,
  account               VARCHAR(40) CHARACTER SET 'utf8' DEFAULT NULL,
  comment               TEXT CHARACTER SET 'utf8' NOT NULL,
  PRIMARY KEY (id)
) Engine=InnoDB CHARACTER SET 'latin1';

CREATE INDEX comments_name_type_idx ON comments (name, type);
CREATE INDEX comments_order_idx ON comments (domain_id, modified_at);


CREATE TABLE domainmetadata (
  id                    INT AUTO_INCREMENT,
  domain_id             INT NOT NULL,
  kind                  VARCHAR(32),
  content               TEXT,
  PRIMARY KEY (id)
) Engine=InnoDB CHARACTER SET 'latin1';

CREATE INDEX domainmetadata_idx ON domainmetadata (domain_id, kind);


CREATE TABLE cryptokeys (
  id                    INT AUTO_INCREMENT,
  domain_id             INT NOT NULL,
  flags                 INT NOT NULL,
  active                BOOL,
  published             BOOL DEFAULT 1,
  content               TEXT,
  PRIMARY KEY(id)
) Engine=InnoDB CHARACTER SET 'latin1';

CREATE INDEX domainidindex ON cryptokeys(domain_id);


CREATE TABLE tsigkeys (
  id                    INT AUTO_INCREMENT,
  name                  VARCHAR(255),
  algorithm             VARCHAR(50),
  secret                VARCHAR(255),
  PRIMARY KEY (id)
) Engine=InnoDB CHARACTER SET 'latin1';

CREATE UNIQUE INDEX namealgoindex ON tsigkeys(name, algorithm);

/*
Using this SQL causes Mysql to create foreign keys on your database. This will
make sure that no records, comments or keys exists for domains that you already
removed. This is not enabled by default, because we're not sure what the
consequences are from a performance point of view. If you do have feedback,
please let us know how this affects your setup.

Please note that it's not possible to apply this, before you cleaned up your
database, as the foreign keys do not exist.
*/
ALTER TABLE records ADD CONSTRAINT `records_domain_id_ibfk` FOREIGN KEY (`domain_id`) REFERENCES `domains` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE comments ADD CONSTRAINT `comments_domain_id_ibfk` FOREIGN KEY (`domain_id`) REFERENCES `domains` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE domainmetadata ADD CONSTRAINT `domainmetadata_domain_id_ibfk` FOREIGN KEY (`domain_id`) REFERENCES `domains` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE cryptokeys ADD CONSTRAINT `cryptokeys_domain_id_ibfk` FOREIGN KEY (`domain_id`) REFERENCES `domains` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

-- ===============================
-- EXAMPLE DOMAINS + RECORDS
-- ===============================

-- Domain 1: example.com
INSERT INTO domains (name, type, account) VALUES ('example.com', 'MASTER', 'test');
SET @domain1_id = LAST_INSERT_ID();

INSERT INTO records (domain_id, name, type, content, ttl, prio)
VALUES
(@domain1_id, 'example.com', 'SOA', 'ns1.example.com. hostmaster.example.com. 2025102901 3600 600 604800 3600', 3600, NULL),
(@domain1_id, 'example.com', 'NS', 'ns1.example.com.', 3600, NULL),
(@domain1_id, 'example.com', 'NS', 'ns2.example.com.', 3600, NULL),
(@domain1_id, 'ns1.example.com', 'A', '192.0.2.53', 3600, NULL),
(@domain1_id, 'ns1.example.com', 'AAAA', '2001:db8::53', 3600, NULL),
(@domain1_id, 'ns2.example.com', 'A', '192.0.2.54', 3600, NULL),
(@domain1_id, 'ns2.example.com', 'AAAA', '2001:db8::54', 3600, NULL),
(@domain1_id, 'example.com', 'A', '192.0.2.1', 3600, NULL),
(@domain1_id, 'example.com', 'AAAA', '2001:db8::1', 3600, NULL),
(@domain1_id, 'mail.example.com', 'A', '192.0.2.10', 3600, NULL),
(@domain1_id, 'example.com', 'MX', 'mail.example.com', 3600, 10),
(@domain1_id, 'www.example.com', 'CNAME', 'example.com', 3600, NULL),
(@domain1_id, 'ftp.example.com', 'CNAME', 'example.com', 3600, NULL),
(@domain1_id, 'example.com', 'TXT', '"v=spf1 include:_spf.example.com ~all"', 3600, NULL),
(@domain1_id, '_spf.example.com', 'TXT', '"v=spf1 ip4:192.0.2.10 -all"', 3600, NULL),
(@domain1_id, '_dmarc.example.com', 'TXT', '"v=DMARC1; p=quarantine; rua=mailto:postmaster@example.com; pct=100"', 3600, NULL),
(@domain1_id, 'default._domainkey.example.com', 'TXT',
 '"v=DKIM1; k=rsa; p=MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAtxM6tS5gG5h6/9eO5u3ncRfYyEjYdT4QFf6Jv7QzO0pS9wHkU+Lx6gZ4a1sR0wJvV9m1bQnKlK0J2X1dDJ1F+5tNQ8V4wHZk6M2YqC8YjQePN9b4cETrYV5Qv+LjfZFGY6n+vTzRok9l8xJpV6T2E8iQF1Z+S5Hp9Yw0Fe9s2q+vY3F+9kPj2u6JfVx8F/5bT5gE1Zc4j6sP8uHzrUoMtKl2GcQXoK+rAxZjx1fQJ+/+NQz+zF2O+9zVqRqK3ykpEbLvq6/2W6t5q9yc9B3El2eCqkF2M7A3Pb1jLx9n9Z6G9ZqDUp+v5S0w1Pt6qB1TzqRj3L+QIDAQAB"',
 3600, NULL);

-- Domain 2: testsite.net
INSERT INTO domains (name, type, account) VALUES ('testsite.net', 'MASTER', 'test');
SET @domain2_id = LAST_INSERT_ID();

INSERT INTO records (domain_id, name, type, content, ttl, prio)
VALUES
(@domain2_id, 'testsite.net', 'SOA', 'ns1.example.com. hostmaster.example.com. 2025102901 3600 600 604800 3600', 3600, NULL),
(@domain2_id, 'testsite.net', 'A', '198.51.100.10', 3600, NULL),
(@domain2_id, 'testsite.net', 'AAAA', '2001:db8:1::10', 3600, NULL),
(@domain2_id, 'www.testsite.net', 'A', '198.51.100.11', 3600, NULL),
(@domain2_id, 'www.testsite.net', 'AAAA', '2001:db8:1::11', 3600, NULL),
(@domain2_id, 'mail.testsite.net', 'A', '198.51.100.12', 3600, NULL),
(@domain2_id, 'mail.testsite.net', 'MX', 'mail.testsite.net', 3600, 10),
(@domain2_id, 'blog.testsite.net', 'CNAME', 'www.testsite.net', 3600, NULL),
(@domain2_id, 'testsite.net', 'TXT', '"v=spf1 include:_spf.testsite.net ~all"', 3600, NULL);

-- Domain 3: mydomain.org
INSERT INTO domains (name, type, account) VALUES ('mydomain.org', 'MASTER', 'test');
SET @domain3_id = LAST_INSERT_ID();

INSERT INTO records (domain_id, name, type, content, ttl, prio)
VALUES
(@domain3_id, 'mydomain.org', 'SOA', 'ns1.example.com. hostmaster.example.com. 2025102901 3600 600 604800 3600', 3600, NULL),
(@domain3_id, 'mydomain.org', 'A', '203.0.113.5', 3600, NULL),
(@domain3_id, 'mydomain.org', 'AAAA', '2001:db8:2::5', 3600, NULL),
(@domain3_id, 'www.mydomain.org', 'A', '203.0.113.6', 3600, NULL),
(@domain3_id, 'www.mydomain.org', 'AAAA', '2001:db8:2::6', 3600, NULL),
(@domain3_id, 'mail.mydomain.org', 'A', '203.0.113.7', 3600, NULL),
(@domain3_id, 'mail.mydomain.org', 'MX', 'mail.mydomain.org', 3600, 10),
(@domain3_id, 'shop.mydomain.org', 'CNAME', 'www.mydomain.org', 3600, NULL),
(@domain3_id, 'mydomain.org', 'TXT', '"v=spf1 include:_spf.mydomain.org ~all"', 3600, NULL);

-- Domain 4: demo.local
INSERT INTO domains (name, type, account) VALUES ('demo.local', 'MASTER', 'test');
SET @domain4_id = LAST_INSERT_ID();

INSERT INTO records (domain_id, name, type, content, ttl, prio)
VALUES
(@domain4_id, 'demo.local', 'SOA', 'ns1.example.com. hostmaster.example.com. 2025102901 3600 600 604800 3600', 3600, NULL),
(@domain4_id, 'demo.local', 'A', '10.0.0.1', 3600, NULL),
(@domain4_id, 'demo.local', 'AAAA', 'fd00::1', 3600, NULL),
(@domain4_id, 'www.demo.local', 'A', '10.0.0.2', 3600, NULL),
(@domain4_id, 'www.demo.local', 'AAAA', 'fd00::2', 3600, NULL),
(@domain4_id, 'mail.demo.local', 'MX', 'mail.demo.local', 3600, 10),
(@domain4_id, 'ftp.demo.local', 'CNAME', 'www.demo.local', 3600, NULL),
(@domain4_id, 'demo.local', 'TXT', '"v=spf1 include:_spf.demo.local ~all"', 3600, NULL);

-- Domain 5: staging.example.net
INSERT INTO domains (name, type, account) VALUES ('staging.example.net', 'MASTER', 'test');
SET @domain5_id = LAST_INSERT_ID();

INSERT INTO records (domain_id, name, type, content, ttl, prio)
VALUES
(@domain5_id, 'staging.example.net', 'SOA', 'ns1.example.com. hostmaster.example.com. 2025102901 3600 600 604800 3600', 3600, NULL),    
(@domain5_id, 'staging.example.net', 'A', '10.1.0.1', 3600, NULL),
(@domain5_id, 'staging.example.net', 'AAAA', 'fd01::1', 3600, NULL),
(@domain5_id, 'www.staging.example.net', 'A', '10.1.0.2', 3600, NULL),
(@domain5_id, 'www.staging.example.net', 'AAAA', 'fd01::2', 3600, NULL),
(@domain5_id, 'mail.staging.example.net', 'MX', 'mail.staging.example.net', 3600, 10),
(@domain5_id, 'blog.staging.example.net', 'CNAME', 'www.staging.example.net', 3600, NULL),
(@domain5_id, 'staging.example.net', 'TXT', '"v=spf1 include:_spf.staging.example.net ~all"', 3600, NULL);
