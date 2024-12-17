<a name="readme-top"></a>
[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)

<div align="center">
<h1 align="center">SharpTimer-Trails</h1>
A plugin that allows the top players (by points) to have a custom trail. Supports <a href="https://github.com/Kandru/cs2-update-manager">automatic updates</a> (optional).
</div>
<br>

> [!IMPORTANT]
> Credits for the base plugin go to [exkludera](https://github.com/exkludera)! All I did was integrate it with SharpTimer, fixed the teleport issue, and added some new config options.
> 
<br>

## Showcase

<img src="https://github.com/user-attachments/assets/1135a673-e19f-4a00-9edc-f4bfc760c45f" width="250">
<img src="https://github.com/user-attachments/assets/af7406b0-3911-489c-91e1-3dde79002790" width="300">
<img src="https://github.com/user-attachments/assets/7dddc6cc-a0aa-4946-9c49-c5bf6b48ceb1" width="200">


<br>

## Information:

### Requirements
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [SharpTimer](https://github.com/Letaryat/poor-sharptimer)

### Roadmap
- [ ] Add configurable trail for replay bot
- [ ] Fix sprites

### Particle Trail Usage
If you want to know which particles you can use for trails, download [Source 2 Viewer](https://github.com/ValveResourceFormat/ValveResourceFormat).
This will allow you view the contents of `pak01_dir.vpk` which has all of the particle files. Not all of them will work, and they are extrememly resource heavy, negiatively effecting client fps. Personally I would advise against using particles trails at this time. Hopefully I can improve this funcitonality in the future.

![image](https://github.com/user-attachments/assets/adaa5452-dab6-4af0-97a6-832453db8e4b)


<br>

## Example Config
- Trail 0 is the trail assigned to anyone who has the permission (e.g. @ccs/root). Trail 1 is assigned to the top player on the server (points), Trail 2 is for the second player on the server, etc.
- You must have a trail configured for each number you have set in your TopCount. So if you have `TopCount: 10`, you should have trails created ranging from 0-10.
- The player rank trail takes presedence over the permission based trail (Trail 0) if a player is both in the top players and has the permission flag. I may change this in the future depending on feedback.
```json
{
  "TopCount": 5,  // The top 5 players with the most points will get a trail.
  "TicksForUpdate": 1,	// How often the trail is updated. The higher the number the less smooth the trail will look.
  "TrailPermission": "@css/vip", // Any player with this perm will get the trail set in Trail 0.
				If you don't want this, just leave it blank.
  "ReloadConfigCommand": "reloadtrailscfg", The command in game to reload your config, useful for testing
  "UpdateConfigCommand": "updatetrailscfg", The command in game to update your config

  "CommandPermission": "@css/root", // Any player with this permission will be able to use the reload/update commands.
  "TeleportThreshold": 100,	// If a user is teleported beyond this many units in a tick the trail won't show.
				This makes it so that there isn't a long straight line between teleports/respawns.
  "DatabaseRefreshInterval": 300,	// How often the plugin fetches the list of top players from the database.
  "DatabaseType": 1,	// 1 = MySQL, 2 = SQLite, 3 = PostgreSQL
  "DatabaseSettings": {
	"Host": "localhost",
	"Database": "cs2_db",
	"Username": "admin",
	"Password": "pw",
	"Port": 3306,
	"Sslmode": "none",
	"Table-Prefix": "",
  }
  "Trails": {
    "0": {
      "Name": "Fire Trail",	// The "Name" field isn't really necessary, you can omit it if you like.
				I left it in so you can easily identify that "Color: 255 165 0" is Orange for example.
      "File": "particles/explosions_fx/molotov_child_flame02a.vpcf"
      "Width": 1,
      "Lifetime": 3
    "1": {
      "Name": "Glowing Sparks Trail",
      "File": "particles/ambient_fx/ambient_sparks_glow.vpcf"
      "Width": 1,
      "Lifetime": 3
    },
    "2": {
      "Name": "Rainbow Trail",
      "Color": "rainbow",
      "Width": 1,
      "Lifetime": 3
    },
    "3": {
      "Name": "Red Trail",
      "Color": "255 0 0",
      "Width": 1,
      "Lifetime": 3
    },
    "4": {
      "Name": "Green Trail",
      "Color": "0 255 0",
      "Width": 1,
      "Lifetime": 3
    },
    "5": {
      "Name": "Blue Trail",
      "Color": "0 0 255",
      "Width": 1,
      "Lifetime": 3
    }
  },
  "EnableDebug": false,	// Set this to true if you find you're having issues with the trail being created when teleporting.
			It will give you insight into how you might want to adjust your TeleportThreshold value, as it shows teleport distances.
  "AutoUpdateConfig": false, // If True, this will automatically update your config when you have installed a version of the plugin (if config values have changed)
  "ConfigVersion": 2
}
```
<p align="right">(<a href="#readme-top">back to top</a>)</p>
