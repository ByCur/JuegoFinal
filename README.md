# Unity Racing Game

A **Unity racing game project** built with C# and Unity 6.

The project combines vehicle physics, car selection, race scenes and arcade-style AI driving. It also integrates third-party Unity assets while adding custom gameplay scripts for menus, controls, spawning and AI behavior.

## 🎮 Features

- Main menu with rules and objective panels
- Car selection with 3D preview
- Persistent selected-car spawning
- Configurable keyboard controls
- Player 1 / Player 2 control support
- Race scene setup
- Waypoint-based AI opponents
- Arcade AI steering and automatic drifting
- Nitro behavior on straights
- Anti-stuck logic for AI vehicles
- Best-time record stored with PlayerPrefs
- Camera follow and preview camera behavior

## 🧠 AI Driving

The custom `AIControllerArcade` script follows a waypoint system and combines:

- steering based on the next waypoint
- speed limiting
- automatic drift behavior
- probabilistic nitro boosts
- recovery logic when the vehicle becomes stuck

## 🛠️ Tech Stack

- **Unity 6** (6000.2.8f1)
- **C#**
- Unity physics / WheelColliders
- TextMesh Pro
- PlayerPrefs
- Scene management

## 📁 Main Custom Scripts

```text
Assets/Scripts/
├── CameraFollow.cs
├── CameraFollow1.cs
├── CarSelectionSingle.cs
├── CarSpawner.cs
├── Controlador.cs
├── IAControllerArcade.cs
├── MainMenu.cs
├── PreviewCamaraOrbit.cs
├── PrometeoCarController.cs
├── PrometeoTouchInput.cs
└── RaceSpawner2P1.cs
```

## 📌 Project Notes

This repository contains a Unity project that uses external vehicle and environment assets alongside custom gameplay code.

The most relevant original work for reviewing the project is located in:

```text
Assets/Scripts/
Assets/Scenes/
```

## 👨‍💻 Author

**Pablo Pérez Arcas**  
Computer Engineering student at the University of Málaga.
