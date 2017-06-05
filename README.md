# Keepass-KeyServer

Keepass-KeyServer is a key provider plugin for KeePass.

# NOT FOR PRODUCTION USE
# DO NOT USE WITH SENSITIVE DATA

The goal is to manage database access over the internet, using a remote "key server".

I'm working with a team of technicians, which copy password databases on their laptop.
I want to control the diffusion of such informations, and eventualy revoke access to a database.
(if a technician leaves the company for example).

The plugin will send informations to the key server to authenticate :
- the computer (using a hardware id)
- the user (using a username / password)
- the location ? (using ip address of the client)

The plugin could also check :
- keepass and plugin version (using certificate thumbprint of keepass and plugin executables for example ?)
- if a validated keepass policy is enforced, to disable features like changing the master key, export and synchronisation features, triggers, ...
