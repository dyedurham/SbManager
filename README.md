SbManager [![Build status](https://ci.appveyor.com/api/projects/status/p9wywmn0so7l2bud?svg=true)](https://ci.appveyor.com/project/brucem/sbmanager) [![Gitter chat](https://badges.gitter.im/globalx/globalx.svg)](https://gitter.im/GlobalX/SbManager) ﻿[![license](https://img.shields.io/badge/license-Apache-brightgreen.svg?style=flat)](https://github.com/GlobalX/SbManager/blob/master/LICENSE)
=========

Installable web ui for managing Azure Service Bus (ASB) and Service Bus for Windows Server (SBWS)

## Overview

SbManager is an app that is installable as a Windows Service (using Topshelf) that allows a level of visibility and maintenence on top of ASB and SBWS. It started life as a project internal to GlobalX Legal Solutions (http://www.globalx.com.au), but is was open-sourced as there was a general feeling that such a tool would be beneficial to the community at large.

While there is already a pretty good tool for doing the things SbManager does, SbManager was created due to the following deficiencies in that existing software:

1. GUI Windows app. This causes issues with remote access to different windows domains requiring RDP access.
2. Usability isn't the best
3. Not properly open-source
4. Some defects (e.g. replaying binary-serialized message bodies). This is resolvable due to the source being available, but see point 3.

Additionally, SBWS has pretty poor monitoring capabilities, so SbManager allows us to expose REST endpoints for monitoring from tools such as PRTG

## Screenshots
(Ignore the temporary logo. In related news: got a good logo?)
![SbManager Overview](http://i.imgur.com/CWS22Jq.png)
![SbManager Subscription](https://cloud.githubusercontent.com/assets/8009243/22172414/c1f532c0-dff1-11e6-9256-5abba7b3ae2a.png)

## Installing/Running

### Local dev/play

1. Open solution in Visual Studio
2. In project 'SbManager' app.config, edit 'connectionStrings'. This can be your connectionstring, or you can point it at a file that contains the connection string (as the app config defaults to)
3. Build. You can run from VS, or run SbManager.exe in the bin folder. This exe uses topshelf so can be installed (see below)

### Packaging and Deploying

//todo There's a lot that can be done here.

At its simplest, build the app (as per above) and run
```
SbManager install
```
See: http://topshelf.readthedocs.org/en/latest/overview/commandline.html

## Using

SbManager has a Web UI and REST endpoints

### Web
The web UI is available by accessing the URL you configured in Installing/Running. This UI should be intuitive to use. If you find anything confusing/clunky, be sure to create an issue and let us know. The best way to keep user guides up to date is to not need them.

### REST
The web UI is powered by the same REST API that you can access for monitoring/integration. No documentation exists yet, but you can see what's available by opening Fiddler/Firebug/etc. while navigating the site and looking at what requests are made. While not ideal, there is some precedent set for this method of 'documentation' (see Discourse @ https://meta.discourse.org/t/about-the-rest-api/3285/3)

Some software can't adequately set HTTP Headers. When consuming the API using such tools, you can control the format of returned data by setting the 'Content-Type' instead of the 'Accept' header. Currently supported overrides are 'application/json' and 'text/xml'

## Security
There are currently NO security features. This is something required, inevitably, for proper use in production scenarios, but for now it does not exist. If you have great ideas on achieving this cleanly, start the discussion.

## Contributing
We'd love everyone and anyone to contribute!

1. Check the issue list for outstanding tasks or create new ones.
2. Send a pull request with completed code and unit/integration tests.
3. Need help? Or support getting your change through? Start a discussion in the issue and if needs be, we can take it offline from there.

## UI Design Tenets
1. Consistent. Although we deal with different messaging entities, things should work the same across all
2. Obvious. It should be clear what everything is and how to use everything. Nothing hidden behind right-clicks or mystery meat
3. Minimal. Don't show things that aren't needed, and don't show things in a complicated way.
4. Responsive. Although desktop is the primary target platform, it should work well on small devices.
