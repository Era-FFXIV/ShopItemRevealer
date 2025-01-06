# Shop Item Revealer
<img src="docs/icon.png" alt="icon" width="50">

This [FFXIVQuickLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) plugin reveals items that are locked while browsing a beast tribe vendor's inventory.

![main screen image](/docs/mainscreen.jpg)

## Features

In addition to revealing the items, it will also tell you how many quests are required to unlock it, based on the reputation needed to reach the rank. Simply hover over the reputation. The quests mentioned are usually unlocked after reaching the cap for a given rank.

![quest image](/docs/questspop.png)

Left clicking the actual item will give you a usable link in your chat window, useful for linking elsewhere.

![link image](/docs/chat.png)

Right-clicking will open a menu, allowing you view details about the requirements on the Wiki or [Garland Tools](https://www.garlandtools.org/db/). Quests use the [Gamer Escape Wiki](https://ffxiv.gamerescape.com/wiki/) and beast tribes use [Console Games Wiki](https://ffxiv.consolegameswiki.com/wiki/).

![right click image](/docs/rightclick.png)

Clicking "Hide Window for This NPC" will make the window not appear when speaking to the currently open vendor. If selected, you can re-reveal it by right-clicking the shop window or with the `/shopitemrevealer` command.

![shop right click](/docs/shopmenu.png)

## Change Log

### 1.0.2 (Current)
 - Adds table sorting by name or quantity
 - Additional assert fixes

### 1.0.1
 - ImGui assert fixes
### 1.0.0
 - Initial release.

## Installing

This is a [FFXIVQuickLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) plugin using Dalamud, so you must install it through that. It should now be in the Plugin Installer as it is now considered stable.

## Contributing

Have any ideas for future updates? Open an Issue here on GitHub and I'll be glad to have a look. 
