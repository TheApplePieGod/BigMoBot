![Logo](logo_small.png)

# Big Mo
A discord bot designed to provide activity tracking, lightweight server management features, and other various fun features. Started as a fun project for my discord server and evolved to be something more generalized.

## Why Mo?
Mo integrates with the fast and powerful SqlServer database platform and has seamless support for multiple discord servers.

## Features
- In-depth user activity tracking
    - Messages sent per user per channel per day
    - Time spent per user per voice channel per day
    - Detailed activity percentage reporting & statistics dumping
    - Chart visualization integration 
- Various administration command such as purge, mute, etc.
- Some other useful features such as reaction roles and custom emotes
- Unique 'hello chain' minigame feature
- High-level customization

## Toggleable Features
- Hello chain (HelloChain)
- Statistics (StatisticsTracking)
- 'Me' command (MeCommand)
- Custom emotes (Emotes)

## Hello Chain?
- Fun minigame where users are tasked to send a unique 'greeting' message in the channel (i.e. hello, good morning, etc.)
- No greeting can be repeated and no user can send two greetings in a row
- If the above happens, the channel is deleted and replaced with a new one
- The person with the most messages when a chain is broken gets a unique 'chain keeper' statistic
- All breaks, keepers, and messages are tracked and can be accessed via various commands
- Be creative!

## Caveats?
- Big Mo is currently a work in progress and there is a lot which is subject to change or not implemented yet
- Big Mo is missing some in-depth customization capability
- Documentation is TBD

## License
Copyright (C) 2021 [Evan Thompson](https://evanthompson.site/)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.