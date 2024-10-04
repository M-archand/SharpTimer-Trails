# SharpTimer-Trails
**A plugin that allows the top players to have a custom trail.**

<br>

<details>
	<summary>Showcase</summary>
	<img src="https://github.com/user-attachments/assets/1135a673-e19f-4a00-9edc-f4bfc760c45f" width="250"> <br>
	<img src="https://github.com/user-attachments/assets/af7406b0-3911-489c-91e1-3dde79002790" width="300"> <br>
	<img src="https://github.com/user-attachments/assets/7dddc6cc-a0aa-4946-9c49-c5bf6b48ceb1" width="200"> <br>
</details>

<br>

## Information:

### Requirements
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [Cruze03/Clientprefs](https://github.com/Cruze03/Clientprefs)

<br>

## Example Config
```json
{
  "Host": "localhost",
  "Database": "cs2_db",
  "Username": "admin",
  "Password": "pw",
  "Port": 3306,
  "Sslmode": "none",
  "Table-Prefix": "",
  "TopCount": 5,
  "Permission": "@css/root",
  "TicksForUpdate": 1,
  "Trails": {
    "1": {
      "Name": "Rainbow Trail",
      "Color": "rainbow"
    },
    "2": {
      "Name": "Particle Trail",
      "File": "particles/ambient_fx/ambient_sparks_glow.vpcf"
    },
    "3": {
      "Name": "Red Trail",
      "Color": "255 0 0",
      "Width": 3,
      "Lifetime": 2
    },
    "4": {
      "Name": "Example Settings",
      "File": "materials/sprites/laserbeam.vtex",
      "Color": "255 255 255",
      "Width": 1,
      "Lifetime": 1
    }
  },
  "ConfigVersion": 1
}
```