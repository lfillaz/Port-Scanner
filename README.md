# Port-Scanner
This tool aims to provide an effective way to examine open ports and assess the security of servers. It can be used for security analysis purposes and enhancing system security settings
![@lfillaz](https://github.com/lfillaz/Port-Scanner/assets/114345508/d421729f-3b19-4f8a-94bc-1af984712f8e)
Data Entry:

The user is asked to enter the IP address of the server they want to scan, as well as a Discord webhook link to send notifications.
Checking ports:

The tool opens connections on all possible ports (1 to 65535) on the given IP address.
Select services:

For each port that is opened, a Python library is used to determine the expected service name associated with that port.
Collection of country information:

The ip-api.com service is used to obtain additional information about the server location such as country, region and city.
Send notifications:

If an open port is found, a notification is sent to the Discord channel with information about the port, its associated service, and a web link to it.
The purpose of the tool is to provide a means for the user to scan servers and determine which ports are open on them, which helps in checking security and verifying network configurations.
_________________________________________________________________________________________________________________
!!Using the tool for any activities related to illegal hacking or illegal cyber attacks is strictly prohibited.!!
