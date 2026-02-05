
# Folder Structle Standart

## Project Root Folders
```Docs:``` Documentation about game and development process.  
```Assets/:``` Main Game Content.  
```Packages/:``` Unity Package Manager.  
```ProjectSettings/:``` Configs.  
```Library/:``` Local cache. 

## Assets Folder Structure

- Assets
  - Scripts
    - Core
    - Player
    - Systems
    - ETC

  - Art
    - Models
    - Textures
    - Materials
    - Shaders
    - Animations
  - Audio
    - Music
    - SFX
  - Prefabs
    - Characters
    - Environment
    - UI
  - Scenes
    - Levels
    - Menus
    - Prototypes
  - UI
  - ScriptableObjects
  - Resources (Speacial Folder - Use with knowladge (```Resources.Load()```))

## Naming Standarts

| Element                    |      Convention      |              Example |
| :------------------------- | :------------------: | -------------------: |
| Scripts                    |      PascalCode      |       PlayerMovement |
| Folders                    |      PascalCode      |        PlayerScripts |
| Classes/Structs            |      PascalCode      |       PlayerMovement |
| Public Fields              |      PascalCase      |            int count |
| Private Fields             |      _camelCase      |             _current |
| Parameters/Local Variables |      camelCase       |                      |
| Constants                  |      PascalCase      | const float MaxSpeed |
| Manager Scripts            | PascalCase + Manager |          GameManager |
| ScriptableObjects          |    SO_PascalCase     |      SO_GameSettings |
| Interfaces                 |    I + PascalCase    |        IInteractable |
| Scene Files                |    SCN_PascalCase    |         SCN_MainMenu |
| Animation Controller       |    AC_PascalCase     |            AC_Player |
| Materials                  |     M_AssetName      |                      |
| Textures                   |     T_AssetName      |                      |
