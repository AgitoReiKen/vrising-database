# vrising-database
BepInEx plugin-library for providing database services to other plugins

```json
{
    "Plugins": {
        "com.agitoreiken.permissions:Persistent": "Persistent",
        "com.agitoreiken.permissions:Local": "Local",
        "com.agitoreiken.localization": "Persistent",
        "com.agitoreiken.playerstats": "Local"
    },
    "DefaultConnector": "Local",
    "EnableMysqlLogger": false,
    "Connectors": {
        "Persistent": {
            "Type": "Mysql",
            "Mysql": {
                "Host": "127.0.0.1",
                "Port": 3306,
                "User": "rei",
                "Password": "1234",
                "Database": "vrising",
                "CreateDatabase": true
            }
        },
        "Local": {
            "Type": "Sqlite",
            "Sqlite": {
                "Path": "BepInEx/config/Database/local.db"
            }
        }
    }
}
```
