# Unity Pinball Game

A physics-based pinball game built with Unity, featuring dynamic paddle controls, particle effects, gravity zones, and a comprehensive scoring system with lives. All game objects and UI elements are programmatically generated at runtime.

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

This is an engaging pinball game where players control two rotating paddles to hit a ball towards green targets. The game features realistic physics interactions, particle effects, gravity zones, ice trails, and a scoring system with lives. The entire game is set up programmatically using scripts, with no manual scene setup required.

## ✨ Features

### Part 1: Physics Interactions
- **Normal Zone** (Left side, gray): Standard friction physics for normal ball movement
- **Ice Zone** (Right side, light blue): Low friction zone that creates a visual trail effect when the ball passes through
- **Gravity Zone** (Left side, orange): A small gravitational field that pulls the ball toward its center - the closer the ball, the stronger the pull
- **Dynamic Physics Materials**: Different zones apply different friction and bounciness values to the ball

### Part 2: Particle Effects
- **Target Collision Particles**: Yellow particle effects trigger when the ball hits green targets
- **Billboard Rendering**: Particles always face the camera for optimal visibility
- **Visual Feedback**: Provides immediate visual feedback for successful target hits
- **Shader Compatibility**: Uses fallback shaders for build compatibility

### Part 3: Input Handling
- **Left Paddle Control**: Press `A` or `Left Arrow` to rotate the left paddle upward
- **Right Paddle Control**: Press `D` or `Right Arrow` to rotate the right paddle upward
- **Rotational Mechanics**: Paddles rotate around their edge pivot points, adding momentum to the ball
- **Ball Launch**: The ball starts on the left paddle and is launched when the paddle rotates
- **Velocity Transfer**: Paddle rotation speed affects ball velocity for dynamic gameplay

### Part 4: Scoring System
- **Target Scoring**: Hitting green targets awards 10 points each
- **Multiple Targets**: 12 targets with random rotations and positions for varied gameplay
- **Dynamic UI**: Score and lives display are automatically generated and updated in real-time
- **Visual Indicators**: Particle effects provide visual confirmation of successful hits
- **Lives System**: Players start with 3 lives; ball loss reduces lives; game over when lives reach 0

## 🎯 Game Mechanics

### Ball Physics
- The ball is constrained to the XZ plane (top-down view) with no Y-axis movement
- Minimum speed (5 units/s) and maximum speed (15 units/s) limits ensure consistent gameplay
- Special collision handling for 90-degree angles to prevent infinite loops
- Realistic reflection physics based on collision angles
- Automatic speed boost if the ball almost stops
- Gravity zone influence: ball trajectory curves toward gravity zone center when nearby

### Paddle System
- Two horizontal paddles positioned on the left and right sides
- Paddles rotate around their edge pivot points when activated
- Rotation speed: 400°/s for responsive gameplay
- Maximum rotation angle: 45°
- Paddles add velocity to the ball based on rotation speed and direction
- Ball-on-paddle detection for initial launch mechanics
- Automatic return to default position when key is released

### Target System
- **12 Targets**: Multiple green cylindrical targets distributed across the play area
- **Random Rotation**: Each target has a random Y-axis rotation (-45° to 45°)
- **Random Position**: Targets have slight position offsets for varied gameplay
- **Scoring**: Each target hit awards 10 points
- **Particle Effects**: Yellow particle burst on collision

### Gravity Zone System
- **Location**: Left side of the play area (orange cylindrical zone)
- **Influence Distance**: 2.5 units maximum range
- **Gravity Strength**: 8 (configurable)
- **Physics**: Uses inverse-square law - closer objects experience stronger gravitational pull
- **Visual**: Orange semi-transparent cylinder for visibility
- **Effect**: Ball trajectory curves toward the zone center when within influence range

### Special Features
- **90-Degree Collision Handling**: Prevents the ball from getting stuck in perpendicular collisions with increasing deflection angles
- **Ice Trail Effect**: Visual trail renderer activates when the ball enters the ice zone
- **Boundary Walls**: Four walls with high bounciness (0.9) keep the ball in play
- **Ball Out Detection**: Automatic detection when ball falls out of play area
- **Life System**: 3 lives per game; ball respawns after loss (if lives remain)
- **Game Over**: Game ends when all lives are lost

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
- **Gravity Calculation**: Inverse-square law for realistic gravitational effects

### Camera System
- **Top-Down Orthographic View**: 90-degree top-down camera angle
- **Optimized View**: Camera positioned at (0, 5, 0) with orthographic size of 4
- **Automatic Setup**: Camera configuration is handled programmatically

### UI System
- **Dynamic Generation**: All UI elements are created via scripts
- **Canvas Setup**: Automatic canvas creation with proper scaling
- **Font Handling**: Uses Unity's built-in `LegacyRuntime.ttf` font for compatibility
- **Real-Time Updates**: Score and lives display update automatically
- **Game Over Screen**: Displays when all lives are lost

### Particle System
- **Billboard Rendering**: Particles use `ParticleSystemRenderMode.Billboard` to face the camera
- **Shader Compatibility**: Uses fallback shaders (`Legacy Shaders/Particles/Alpha Blended`, `Sprites/Default`, `Unlit/Color`) for build compatibility
- **Burst Emission**: Particles emit in bursts on collision
- **Color**: Yellow particles for target hits

### Gravity Zone System
- **Distance-Based Force**: Gravity strength calculated using inverse-square law
- **XZ Plane Calculation**: All gravity calculations performed on XZ plane
- **Real-Time Detection**: Continuous checking for ball position within influence range
- **Force Application**: Gravity force applied to ball velocity in `FixedUpdate`

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── PinballGameSetup.cs      # Main controller - creates all game objects
│   ├── PinballGameManager.cs    # Game state management, lives, ball respawn
│   ├── BallController.cs         # Ball physics, speed limits, collision handling, gravity
│   ├── BallOutDetector.cs       # Detects when ball falls out of play area
│   ├── PaddleController.cs       # Paddle rotation and ball interaction
│   ├── ScoreManager.cs           # Score management system
│   ├── ParticleTrigger.cs       # Particle effect trigger on target hits
│   ├── PhysicsZone.cs           # Zone physics material application
│   ├── GravityZone.cs           # Gravity zone with inverse-square law physics
│   └── UIManager.cs             # Dynamic UI creation and management
└── Editor/
    └── TagSetup.cs              # Automatic tag setup for Unity editor
```

### Script Descriptions

#### `PinballGameSetup.cs`
- Main setup script that initializes all game components
- Creates game objects programmatically (table, ball, paddles, targets, walls, zones)
- Sets up camera, UI, and physics zones
- Handles material and shader setup with fallback mechanisms
- Creates 12 targets with random rotations and positions
- Sets up gravity zone on the left side

#### `PinballGameManager.cs`
- Manages game state and flow
- Handles lives system (starts with 3 lives)
- Manages ball respawn after loss
- Detects game over condition
- Coordinates with UI manager for lives display

#### `BallController.cs`
- Manages ball movement and speed constraints
- Enforces minimum (5) and maximum (15) speed limits
- Handles special 90-degree collision deflection
- Manages trail renderer for ice zone effects
- Keeps ball constrained to XZ plane
- Applies gravity zone forces using `ApplyGravityZones()` method
- Prevents ball from getting stuck with automatic speed boost

#### `BallOutDetector.cs`
- Detects when the ball falls out of the play area
- Triggers game manager's `OnBallOut()` method
- Uses trigger collider to detect ball exit

#### `PaddleController.cs`
- Controls paddle rotation around edge pivot points
- Detects ball-on-paddle state for initial launch
- Calculates ball reflection with paddle velocity
- Handles input for left and right paddles
- Rotation speed: 400°/s, max angle: 45°
- Automatic return to default position

#### `GravityZone.cs`
- Defines gravity zone properties and behavior
- Calculates gravitational force using inverse-square law
- Provides methods for distance and force calculations
- Visual representation with orange semi-transparent material
- Configurable gravity strength and influence distance

#### `ScoreManager.cs`
- Manages game score
- Provides methods to add and retrieve score
- Communicates with UI manager for display updates

#### `ParticleTrigger.cs`
- Triggers particle effects on target collisions
- Sets up particle system with proper rendering mode
- Ensures particles face the camera
- Awards points on target hit

#### `PhysicsZone.cs`
- Applies different physics materials to zones
- Manages ice zone trail effect activation
- Handles zone entry and exit events
- Supports normal and ice zones with different friction values

#### `UIManager.cs`
- Dynamically creates canvas and UI elements
- Manages score display, lives display, and instructions
- Handles UI updates during gameplay
- Displays game over message

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

- **Camera**: Top-down orthographic view (90° angle)
- **Play Area**: 5x4 units (XZ plane)
- **Ball**: Red sphere, constrained to XZ plane
- **Paddles**: Blue horizontal paddles on left and right
- **Targets**: 12 green cylindrical targets with random rotation and position
- **Walls**: White boundary walls with high bounciness (0.9)
- **Zones**: 
  - Gray (normal) - standard friction
  - Light blue (ice) - low friction with trail effect
  - Orange (gravity) - gravitational pull toward center

## 🎯 Gameplay Tips

1. **Use Gravity Zone**: When the ball approaches the orange gravity zone, its trajectory will curve toward the center - use this to your advantage for scoring
2. **Paddle Timing**: Time your paddle rotations to maximize ball velocity
3. **Target Strategy**: Aim for targets in the center area for easier hits
4. **Ice Zone**: The ice zone reduces friction - use it to maintain ball speed
5. **Life Management**: You have 3 lives - be careful not to let the ball fall out!

## 🐛 Known Issues & Solutions

### Issue: Purple Materials in Build
**Solution**: The project uses fallback shaders (`Legacy Shaders/Diffuse` or `Unlit/Color`) to ensure compatibility.

### Issue: Ball Gets Stuck in 90-Degree Collisions
**Solution**: Special deflection logic handles perpendicular collisions with increasing deflection angles.

### Issue: Particles Not Visible
**Solution**: Particles use Billboard rendering mode and are configured to face the camera.

### Issue: Ball Flies Out of Bounds
**Solution**: Ball is constrained to XZ plane, and boundary walls prevent escape. Ball out detector handles cases where ball falls through opening.

## 📝 License

This project is open source and available for educational purposes.

## 👤 Author

Created as a Unity game development project.

## 🔗 Repository

GitHub: https://github.com/kriofeng/Unity-Pinball-Game

---

**Enjoy playing the game!** 🎮
