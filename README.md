# 🏎️ Unity Racing Game

A 3D **arcade-style racing game built with Unity and C#**.

The project includes custom gameplay systems for vehicle control, car selection, race spawning, menus, camera behavior and AI-controlled opponents. It combines Unity vehicle physics with waypoint-based driving logic and external environment/vehicle assets.

---

## 🎮 Main Features

- Main menu with rules and objective screens
- 3D car selection with preview camera
- Persistent selected-car data using `PlayerPrefs`
- Vehicle spawning at race start
- Configurable keyboard controls
- Support for separate Player 1 / Player 2 key mappings
- WheelCollider-based vehicle physics
- Camera follow and preview camera systems
- Best-time record saved between sessions
- Waypoint-based AI opponents
- Automatic AI drifting
- Nitro boosts on straights
- Anti-stuck recovery logic for AI vehicles

---

## 🧠 Arcade AI Opponents

One of the main custom systems in the project is `AIControllerArcade.cs`.

The AI follows a waypoint route and adjusts its behavior according to the direction of the next waypoint.

The controller includes:

- waypoint navigation
- steering correction
- configurable maximum speed
- automatic drift activation in tighter turns
- temporary nitro boosts on straights
- recovery behavior when the car becomes stuck

Simplified flow:

```text
Current vehicle position
        ↓
Next waypoint
        ↓
Calculate local direction
        ↓
Steering + acceleration
        ↓
Turn detected?
   ┌────┴────┐
  Yes        No
   ↓          ↓
 Drift    Normal grip
        ↓
Straight section?
        ↓
Possible nitro boost
```

---

## 🚗 Vehicle Control

The custom `Controlador.cs` script manages the main vehicle behavior.

It includes:

- acceleration and reverse
- steering
- braking
- handbrake
- configurable key mappings
- WheelCollider interaction
- speed calculation
- drift / traction behavior
- optional particle, skid and sound effects

Default keyboard mappings are configured separately for Player 1 and Player 2 when no custom keys are assigned in the Unity Inspector.

---

## 🧩 Game Flow

The main gameplay flow is structured around several Unity scenes and scripts:

```text
Main Menu
    ↓
Car Selection
    ↓
Selected vehicle stored
    ↓
Race Scene
    ↓
Vehicle spawned
    ↓
Player / AI driving
```

Relevant scenes currently included in the project:

```text
Assets/Scenes/
├── MainMenu.unity
├── CarSelectScene.unity
├── Race.unity
├── TRACK.unity
└── SampleScene.unity
```

---

## 🛠️ Tech Stack

- **Unity 6** — 6000.2.8f1
- **C#**
- Unity Physics
- WheelColliders
- TextMesh Pro
- PlayerPrefs
- Scene Management
- Unity UI

The project also includes Unity packages for navigation, rendering, testing and editor integration.

---

## 📁 Project Structure

```text
JuegoFinal/
│
├── Assets/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── CameraFollow.cs
│   │   ├── CameraFollow1.cs
│   │   ├── CarSelectionSingle.cs
│   │   ├── CarSpawner.cs
│   │   ├── Controlador.cs
│   │   ├── IAControllerArcade.cs
│   │   ├── MainMenu.cs
│   │   ├── PreviewCamaraOrbit.cs
│   │   ├── PrometeoCarController.cs
│   │   ├── PrometeoTouchInput.cs
│   │   └── RaceSpawner2P1.cs
│   │
│   └── ...
│
├── Packages/
│   └── manifest.json
│
├── ProjectSettings/
│
└── README.md
```

---

## 🔑 Main Custom Scripts

### `MainMenu.cs`
Controls the main menu, rules/objective panels and saved best-time display.

### `CarSelectionSingle.cs`
Handles car selection, vehicle preview and storing the selected car before loading the race scene.

### `CarSpawner.cs`
Loads the selected vehicle index from `PlayerPrefs` and instantiates the correct prefab at the race spawn point.

### `Controlador.cs`
Main custom vehicle controller with acceleration, steering, braking, handbrake and configurable controls.

### `IAControllerArcade.cs`
Controls AI opponents using waypoints, steering logic, drift behavior, nitro and anti-stuck recovery.

### `RaceSpawner2P1.cs`
Contains logic for spawning two selected vehicles at separate starting positions.

---

## ▶️ Running the Project

### Requirements

- Unity Hub
- **Unity 6000.2.8f1** or a compatible Unity 6 version

### Steps

1. Clone the repository:

```bash
git clone https://github.com/ByCur/JuegoFinal.git
```

2. Open **Unity Hub**.

3. Select **Add project from disk**.

4. Choose the cloned `JuegoFinal` folder.

5. Open the project with Unity 6.

6. Load the main menu or race scene from:

```text
Assets/Scenes/
```

> Unity may need some time on the first launch to import the project assets and packages.

---

## 📦 Assets

This project uses third-party Unity assets for parts of the vehicles, tracks, environment and supporting systems.

The main custom gameplay implementation can be reviewed in:

```text
Assets/Scripts/
Assets/Scenes/
```

---

## 📚 What This Project Demonstrates

This project gave me practical experience with:

- C# gameplay programming
- Unity scene management
- object instantiation and prefab workflows
- vehicle physics
- WheelColliders
- configurable player input
- persistent game state with PlayerPrefs
- camera systems
- waypoint-based game AI
- arcade driving behavior
- debugging and integrating third-party Unity assets

---

## 👨‍💻 Authors

**Pablo Pérez Arcas**
**And Juan José Soler Gordo**

Computers Engineering students at the **University of Málaga (UMA)**.

Interested in **Software Engineering, Artificial Intelligence and interactive systems**.
