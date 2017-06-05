# Server part

This is the server part. This part is hosted on a web server, and is used to authenticate the user and the device trying to open a protected database.

For testing purpose, i run this part on Cassini Web Server.
(https://cassinidev.codeplex.com/)
I use it with the following parameters :
 - Physical Path = root of KeyServer folder
 - VirtualPath = /
 - Port = Specific / 80
 - HostName = (empty)
 - Options : NTLM Authentication Required

The file "Permissions.xml" contains hardwareid and usernames allowed to access a database.
The file "Keys.xml" contains database names and keys.
The file "Requests.xml" logs keys requests.

# THIS CODE IS ONLY FOR TESTING PURPOSES.
# DO NOT USE PRODUCTION DATA.
