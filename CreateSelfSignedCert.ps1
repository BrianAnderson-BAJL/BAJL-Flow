$cert = New-SelfSignedCertificate -DnsName flowenginedev.bajlllc.com -CertStoreLocation cert:\LocalMachine\My

$pwd = ConvertTo-SecureString -String "MyPassword" -Force -AsPlainText

Export-PfxCertificate -Cert $cert -FilePath C:\GameDev\self_signed_cert.pfx -Password $pwd