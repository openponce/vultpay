PASSOS PARA GERAÇÃO DE UM CERTIFICADO VÁLIDO

--------------------------------------------
Certificate Authority
--------------------------------------------

openssl genrsa -out certificate_authority/vultpay_ca_root.key 4096

openssl req -x509 -new -nodes -key certificate_authority/vultpay_ca_root.key -out certificate_authority/vultpay_ca_root.crt -days 3650

openssl pkcs12 -export -password pass:EA03D3FBF43D44BCBA1408AAB56261D6 -out certificate_authority/vultpay_ca_root.pfx -inkey certificate_authority/vultpay_ca_root.key -in certificate_authority/vultpay_ca_root.crt

--------------------------------------------
Certificate Authority Password
--------------------------------------------
EA03D3FBF43D44BCBA1408AAB56261D6
--------------------------------------------

--------------------------------------------
Certificate Server
--------------------------------------------

openssl req -newkey rsa:2048 -nodes -keyout server/vultpay_server.key -subj "/C=BR/ST=SP/L=SP/O=localhost/CN=*.localhost" -out server/vultpay_server.csr

openssl x509 -req -extfile "server.v3.conf" -days 3650 -in server/vultpay_server.csr -CA certificate_authority/vultpay_ca_root.crt -CAkey certificate_authority/vultpay_ca_root.key -CAcreateserial -out server/vultpay_server.crt

openssl pkcs12 -export -password pass:B76ADC7F261246F9A3B30512A4CA16B6 -out server/vultpay_server.pfx -inkey server/vultpay_server.key -in server/vultpay_server.crt

--------------------------------------------
Certificate Server Password
--------------------------------------------
B76ADC7F261246F9A3B30512A4CA16B6
--------------------------------------------

--------------------------------------------
Certificate Bundle
--------------------------------------------

cat server/vultpay_server.crt certificate_authority/vultpay_ca_root.crt > vultpay_ssl_bundle.crt

--------------------------------------------
Adicionar certificado raiz no windows
--------------------------------------------

certutil –addstore -enterprise –f "Root" cert.pem