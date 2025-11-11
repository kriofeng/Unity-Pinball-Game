# Unity Pinball Game

A physics-based pinball game built with Unity, featuring dynamic paddle controls, particle effects, and a scoring system. All game objects and UI elements are programmatically generated at runtime.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Game Mechanics](#game-mechanics)
- [Controls](#controls)
- [Technical Implementation](#technical-implementation)
- [Project Structure](#project-structure)
- [Installation & Setup](#installation--setup)
- [Requirements](#requirements)
- [License](#license)

## 🎮 Overview

This is a simple yet engaging pinball game where players control two rotating paddles to hit a ball towards green targets. The game features realistic physics interactions, particle effects, and a scoring system. The entire game is set up programmatically using scripts, with no manual scene setup required.

## ✨ Features

### Part 1: Physics Interactions
- **Normal Zone** (Left side, gray): Standard friction physics for normal ball movement
- **Ice Zone** (Right side, light blue): Low friction zone that creates a visual trail effect when the ball passes through
- **Dynamic Physics Materials**: Different zones apply different friction and bounciness values to the ball

### Part 2: Particle Effects
- **Target Collision Particles**: Yellow particle effects trigger when the ball hits green targets
- **Billboard Rendering**: Particles always face the camera for optimal visibility
- **Visual Feedback**: Provides immediate visual feedback for successful target hits

### Part 3: Input Handling
- **Left Paddle Control**: Press `A` or `Left Arrow` to rotate the left paddle upward
- **Right Paddle Control**: Press `D` or `Right Arrow` to rotate the right paddle upward
- **Rotational Mechanics**: Paddles rotate around their edge pivot points, adding momentum to the ball
- **Ball Launch**: The ball starts on the left paddle and is launched when the paddle rotates

### Part 4: Scoring System
- **Target Scoring**: Hitting green targets awards 10 points
- **Dynamic UI**: Score display is automatically generated and updated in real-time
- **Visual Indicators**: Particle effects provide visual confirmation of successful hits

## 🎯 Game Mechanics

### Ball Physics
- The ball is constrained to the XZ plane (top-down view) with no Y-axis movement
- Minimum and maximum speed limits ensure consistent gameplay
- Special collision handling for 90-degree angles to prevent infinite loops
- Realistic reflection physics based on collision angles

### Paddle System
- Two horizontal paddles positioned on the left and right sides
- Paddles rotate around their edge points when activated
- Rotation speed and force are optimized for responsive gameplay
- Paddles add velocity to the ball based on rotation speed and direction

### Target System
- Three green targets with random Y-axis rotation (-45° to 45°)
- Random position offsets for varied gameplay
- Each target triggers particle effects and awards points on collision

### Special Features
- **90-Degree Collision Handling**: Prevents the ball from getting stuck in perpendicular collisions
- **Ice Trail Effect**: Visual trail renderer activates when the ball enters the ice zone
- **Boundary Walls**: Four walls with high bounciness keep the ball in play

## 🎮 Controls

| Action | Key |
|--------|-----|
| Rotate Left Paddle | `A` or `←` (Left Arrow) |
| Rotate Right Paddle | `D` or `→` (Right Arrow) |

**Note**: Hold the key to continuously rotate the paddle. Release to let it return to the default position.

## 🔧 Technical Implementation

### Physics System
- **Rigidbody Constraints**: Ball is constrained to XZ plane using `FreezePositionY` and rotation constraints
- **Collision Detection**: Continuous collision detection mode for accurate physics
- **PhysicMaterials**: Custom physics materials for different zones and objects
- **Kinematic Rigidbodies**: Paddles use kinematic rigidbodies for precise control

### Camera System
- **Top-Down Orthographic View**: 90-degree top-down camera angle
- **Optimized View**: Camera positioned at (0, 5, 0) with orthographic size of 4
- **Automatic Setup**: Camera configuration is handled programmatically

### UI System
- **Dynamic Generation**: All UI elements are created via scripts
- **Canvas Setup**: Automatic canvas creation with proper scaling
- **Font Handling**: Uses Unity's built-in `LegacyRuntime.ttf` font for compatibility
- **Real-Time Updates**: Score display updates automatically

### Particle System
- **Billboard Rendering**: Particles use `ParticleSystemRenderMode.Billboard` to face the camera
- **Shader Compatibility**: Uses fallback shaders for build compatibility
- **Burst Emission**: Particles emit in bursts on collision

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── PinballGameSetup.cs      # Main controller - creates all game objects
│   ├── BallController.cs         # Ball physics, speed limits, collision handling
│   ├── PaddleController.cs       # Paddle rotation and ball interaction
│   ├── ScoreManager.cs           # Score management system
│   ├── ParticleTrigger.cs       # Particle effect trigger on target hits
│   ├── PhysicsZone.cs           # Zone physics material application
│   └── UIManager.cs             # Dynamic UI creation and management
└── Editor/
    └── TagSetup.cs              # Automatic tag setup for Unity editor
```

### Script Descriptions

#### `PinballGameSetup.cs`
- Main setup script that initializes all game components
- Creates game objects programmatically (table, ball, paddles, targets, walls)
- Sets up camera, UI, and physics zones
- Handles material and shader setup with fallback mechanisms

#### `BallController.cs`
- Manages ball movement and speed constraints
- Enforces minimum and maximum speed limits
- Handles special 90-degree collision deflection
- Manages trail renderer for ice zone effects
- Keeps ball constrained to XZ plane

#### `PaddleController.cs`
- Controls paddle rotation around edge pivot points
- Detects ball-on-paddle state for initial launch
- Calculates ball reflection with paddle velocity
- Handles input for left and right paddles

#### `ScoreManager.cs`
- Manages game score
- Provides methods to add and retrieve score
- Communicates with UI manager for display updates

#### `ParticleTrigger.cs`
- Triggers particle effects on target collisions
- Sets up particle system with proper rendering mode
- Ensures particles face the camera

#### `PhysicsZone.cs`
- Applies different physics materials to zones
- Manages ice zone trail effect activation
- Handles zone entry and exit events

#### `UIManager.cs`
- Dynamically creates canvas and UI elements
- Manages score display and instructions
- Handles UI updates during gameplay

## 🚀 Installation & Setup

### Prerequisites
- Unity 2021.3 or later
- No additional packages required (uses built-in Unity features)

### Setup Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/kriofeng/Unity-Pinball-Game.git
   ```

2. **Open in Unity**
   - Open Unity Hub
   - Click "Add" and select the cloned project folder
   - Open the project with Unity 2021.3 or later

3. **Run the Game**
   - The game automatically sets up when you press Play
   - No manual scene setup is required
   - All game objects, UI, and physics are created via scripts

4. **Optional: Create GameManager**
   - If needed, create an empty GameObject named "GameManager"
   - Attach the `PinballGameSetup` script to it
   - However, the script will work even without this step as it creates objects programmatically

### Build Instructions

1. Go to `File > Build Settings`
2. Select your target platform (Windows, Mac, Linux, etc.)
3. Click "Build"
4. The game uses fallback shaders for compatibility across different platforms

## 📋 Requirements

- **Unity Version**: 2021.3 or later
- **Platform**: Windows, Mac, or Linux (tested on Windows)
- **Dependencies**: None (uses only Unity built-in features)

## 🎨 Game View

- **Camera**: Top-down orthographic view
- **Play Area**: 5x4 units (XZ plane)
- **Ball**: Red sphere, constrained to XZ plane
- **Paddles**: Blue horizontal paddles on left and right
- **Targets**: Green cubes with random rotation and position
- **Walls**: White boundary walls with high bounciness
- **Zones**: Gray (normal) and light blue (ice) physics zones

## 🐛 Known Issues & Solutions

### Issue: Purple Materials in Build
**Solution**: The project uses fallback shaders (`Legacy Shaders/Diffuse` or `Unlit/Color`) to ensure compatibility.

### Issue: Ball Gets Stuck in 90-Degree Collisions
**Solution**: Special deflection logic handles perpendicular collisions with increasing deflection angles.

### Issue: Particles Not Visible
**Solution**: Particles use Billboard rendering mode and are configured to face the camera.

## 📝 License

This project is open source and available for educational purposes.

## 👤 Author

Created as a Unity game development project.

## 🔗 Repository

GitHub: https://github.com/kriofeng/Unity-Pinball-Game

---

**Enjoy playing the game!** 🎮
