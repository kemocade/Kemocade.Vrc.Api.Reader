# VRChat API Reader

[![VPM Package Version](https://img.shields.io/vpm/v/com.kemocade.vrc.api.reader?repository_url=https%3A%2F%2Fkemocade.github.io%2FKemocade.Vrc.Api.Reader%2Findex.json)](https://kemocade.github.io/Kemocade.Vrc.Api.Reader)
[![Code Coverage](https://kemocade.github.io/Kemocade.Vrc.Api.Reader/coverage/badge_shieldsio_linecoverage_brightgreen.svg)](https://kemocade.github.io/Kemocade.Vrc.Api.Reader/coverage)

-----
# Introduction

This package exposes various information from the [VRChat API](https://vrchatapi.github.io) to [VRChat's Udon system](https://creators.vrchat.com/worlds/udon), where it can be used in the creation of VRChat Worlds.

This information includes VRChat World stats such as visits and current occcupants, as well as user membership/roles from [VRChat Groups](https://help.vrchat.com/hc/articles/11706395001875-Creating-a-Group) and/or [Discord Servers](https://support.discord.com/hc/articles/204849977-How-do-I-create-a-server), all of which are not otherwise possible to access within Udon.
By syncing Discord roles with third-party platforms such as [Patreon](https://support.patreon.com/hc/articles/212052266-Getting-Discord-access) or [Gumroad](https://help.gumroad.com/article/295-external-integration#discord), VRChat world creators can use this system to automatically provide in-world incentives to users who support their work financially.

The package accomplishes this by pairing with a companion GitHub Actions workflow (the [VRChat Api Tracker](https://github.com/kemocade/Kemocade.Vrc.Api.Tracker) system), which uses the [VRChat API](https://vrchatapi.github.io) and/or a [Discord Bot](https://discord.com/developers/docs/intro) to gather the desired data.
That information is published to [GitHub Pages](https://pages.github.com/), where it can then be consumed by this package via [Remote String Loading](https://creators.vrchat.com/worlds/udon/string-loading).
A template repository for quickly and easily configuring this workflow is provided in the instructions below.

# Videos

## Overview
[![Overview Video](http://img.youtube.com/vi/844vgLt39Nc/0.jpg)](http://www.youtube.com/watch?v=844vgLt39Nc "Overview")

## Step-by-Step Tutorial
***Coming Soon!***

# Prerequisites
Before using this package, you must configure your own instance of the [VRChat Api Tracker](https://github.com/kemocade/Kemocade.Vrc.Api.Tracker) system.
Visit this template repository and create your own repository from it, then following the instructions in the README to configure tracking of your desired information.
Afterwards, you can return to this package and use it to access the tracked data in your VRChat World.

# Installation
Install via the [VCC Package Listing](https://kemocade.github.io/Kemocade.Vrc.Api.Reader).

## Usage
Add the provided [Reader](Packages/com.kemocade.vrc.api.reader/Runtime/Reader.cs) `Component` to any `GameObject` in your scene.
Connect to your tracked data by using the inspector to set the `Data Url` property to your GitHub Pages `data.json` Url from the [final step of configuring your VRChat Role Tracker](https://github.com/kemocade/Kemocade.Vrc.Role.Tracker#7-get-the-results) instance.

If you want to execute code immediately upon this loading process completing (or any future data refreshes), see [Inheritance](#inheritance).

For a full list of available methods, see [API](#api).

## Configuring Automatic Data Refreshes
You can configure the frequency at which the downloaded data will be refreshed by modifying the `Hours To Wait After Update` and `Minutes To Wait Between Refreshes` fields in the `Reader`'s inspector.

`Hours To Wait After Update` is set to a default of `1` hour.
This is how long your `Reader` will wait after the most recent time your `data.json` file was updated, which is stored in the `fileTimeUtc` JSON property.
Because the [VRChat Api Tracker](https://github.com/kemocade/Kemocade.Vrc.Api.Tracker) system is set to run once an hour by default, this default configuration will match that rate and avoid checking for updates until an hour has passed.

`Minutes To Wait Between Refreshes` is set to a default of `5` minutes.
This is how long your `Reader` will wait after its most recent attempt to refresh your data.
These refresh attempts will only start occuring after the previous `Hours To Wait After Update` condition has been satisfied.
For example, with both settings on default, the `Reader` will wait 1 hour after a `data.json` update to start checking for new data.
It will then do this every 5 minutes until new data is found.
After new data is found, it will wait another hour before repeating this process indefinitely.

`Reader` provides a `bool` property named `IsReady` which determines if the remote string loading process has been completed.
All of the following methods will return `false` until this process has finished, so you should always check `IsReady` before using them.

## Inheritance

You can make your own [subclass](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/inheritance) of the `Reader` component to hook into the remote string loading process and execute code as soon as the data is ready.
This is useful for quickly cacheing data to be used later, such as whether or not the local user is a Moderator.

Here is an example of a `Reader` subclass named `ReaderTest` which logs whether or not the local user is a Moderator as soon as the `Reader` is ready:

```csharp
using Kemocade.Vrc.Api.Reader;
using UnityEngine;

public class ReaderTest : Reader
{
    public override void OnRefresh() => Debug.Log(IsModeratorAnywhereLocal());
}
```

## API

### Utility
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `Refresh` | | `void` | Manually force the remote data to be refreshed |
| `IsReady` | | `bool` | If remote data has successfully been downloaded at least one time and is ready to be accessed |

### Metadata
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `TryGetFileTimeUtc` | `out long fileTimeUtc` | `bool` | If a valid `fileTimeUtc` could be found in the downloaded data |
| `TryGetDateTime` | `out DateTime dateTime` | `bool` | If a valid `DateTime` could be parsed from the downloaded data |

### VRC World Data
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `TryGetVrcWorldName` | `out string name`, `string vrcWorldId` | `bool` | If a valid `name` could be found for the given VRC World |
| `TryGetVrcWorldVisits` | `out int visits`, `string vrcWorldId` | `bool` | If a valid `visits` count could be found for the given VRC World |
| `TryGetVrcWorldFavorites` | `out int favorites`, `string vrcWorldId` | `bool` | If a valid `favorites` count could be found for the given VRC World |
| `TryGetVrcWorldOccupants` | `out int occupants`, `string vrcWorldId` | `bool` | If a valid `occupants` count could be found for the given VRC World |

### VRC Group Data
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `TryGetVrcGroupName` | `out string name`, `string vrcGroupId` | `bool` | If a valid `name` could be found for the given VRC Group |
| `TryGetVrcGroupMemberCount` | `out int memberCount`, `string vrcGroupId` | `bool` | If a valid `memberCount` could be found for the given VRC Group |
| `TryGetDisplayNamesFromVrcGroupMembers` | `out string[] displayNames`, `string vrcGroupId` | `bool` | If valid `displayNames` could be found for the given VRC Group |
| `TryGetDisplayNamesFromVrcGroupMembersWithRole` | `out string[] displayNames`, `string vrcGroupId`, `string vrcGroupRoleId` | `bool` | If valid `displayNames` could be found for the given role in the given VRC Group |

### VRC Group Member Data by Display Name
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `IsVrcGroupMember` | `string name`, `string vrcGroupId` | `bool` | If the given user is a member of the given VRC Group |
| `HasVrcGroupRole` | `string name`, `string vrcGroupId`, `string vrcGroupRoleId` | `bool` | If the given user has the given role in the given VRC Group |
| `IsVrcGroupAdmin` | `string name`, `string vrcGroupId` | `bool` | If the given user is the owner of the given VRC Group |
| `IsVrcGroupModerator` | `string name`, `string vrcGroupId` | `bool` | If the given user has any role with instance moderation permissions in the VRC Group |

### VRC Group Member Data for Local User
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `IsVrcGroupMemberLocal` | `string vrcGroupId` | `bool` | If the local user is a member of the given VRC Group |
| `HasVrcGroupRoleLocal` | `string vrcGroupId`, `string vrcGroupRoleId` | `bool` | If the local user has the given role in the given VRC Group |
| `IsVrcGroupAdminLocal` | `string vrcGroupId` | `bool` | If the local user is the owner of the given VRC Group |
| `IsVrcGroupModeratorLocal` | `string vrcGroupId` | `bool` | If the local user has any role with instance moderation permissions in the VRC Group |

### Discord Server Data
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `TryGetDiscordServerName` | `out string name`, `string discordGuildId` | `bool` | If a valid `name` could be found for the given Discord Server |
| `TryGetDiscordServerMemberCount` | `out int memberCount`, `string discordGuildId` | `bool` | If a valid `memberCount` could be found for the given Discord Server |
| `TryGetDiscordVrcLinkedMemberCount` | `out int vrcLinkedMemberCount`, `string discordGuildId` | `bool` | If a valid `memberCount` could be found for the given Discord Server's members who have been linked with VRC users |
| `TryGetDisplayNamesFromDiscordVrcLinkedMembers` | `out string[] displayNames`, `string discordGuildId` | `bool` | If valid `displayNames` could be found for the given Discord Server's members who have been linked with VRC users |
| `TryGetDisplayNamesFromDiscordVrcLinkedMembersWithRole` | `out string[] displayNames`, `string discordGuildId`, `string discordRoleId` | `bool` | If valid `displayNames` could be found for the given role in the given Discord Server's members who have been linked with VRC users |

### Discord Server Member Data by Display Name
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `IsDiscordMember` | `string name`, `string discordGuildId` | `bool` | If the given user is a member of the given Discord Server |
| `HasDiscordRole` | `string name`, `string discordGuildId`, `string discordRoleId` | `bool` | If the given user has the given role in the given Discord Server |
| `IsDiscordAdmin` | `string name`, `string discordGuildId` | `bool` | If the given user has any role with Admin permissions in the given Discord Server |
| `IsDiscordModerator` | `string name`, `string discordGuildId` | `bool` | If the given user has any role with Moderation permissions in the given Discord Server |

### Discord Server Member Data for Local User
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `IsDiscordMemberLocal` | `string discordGuildId` | `bool` | If the local user is a member of the given Discord Server |
| `HasDiscordRoleLocal` | `string discordGuildId`, `string discordRoleId` | `bool` | If the local user has the given role in the given Discord Server |
| `IsDiscordAdminLocal` | `string discordGuildId` | `bool` | If the local user has any role with Admin permissions in the given Discord Server |
| `IsDiscordModeratorLocal` | `string discordGuildId` | `bool` | If the local user has any role with Moderation permissions in the given Discord Server |

### Combined Data by Display Name
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `IsAdminAnywhere` | `string name` | `bool` | If the given user has any role with Admin permissions across any tracked VRC Groups or Discord Servers |
| `IsModeratorAnywhere` | `string name` | `bool` | If the given user has any role with Moderation permissions across any tracked VRC Groups or Discord Servers |

### Combined Data for Local User
| **Method** | **Parameters** | **Returns** | **Description** |
| - | - | - | - |
| `IsAdminAnywhereLocal` | | `bool` | If the local user has any role with Admin permissions across any tracked VRC Groups or Discord Servers |
| `IsModeratorAnywhereLocal` | | `bool` | If the local user has any role with Moderation permissions across any tracked VRC Groups or Discord Servers |
