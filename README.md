# Unity Pinball Game

A simple pinball game built with Unity, featuring physics-based gameplay, particle effects, and dynamic paddle controls.

## Features

- **Physics-based ball movement**: Ball moves on a 2D plane (XZ) with realistic physics
- **Rotating paddles**: Left and right paddles that rotate to hit the ball
- **Target scoring**: Green targets that trigger particle effects and add score
- **Ice zone**: Special zone with low friction that creates a trail effect
- **Dynamic UI**: All UI elements are generated via scripts
- **90-degree collision handling**: Special deflection mechanism to prevent infinite loops

## Controls

- **A / Left Arrow**: Control left paddle (rotate upward)
- **D / Right Arrow**: Control right paddle (rotate upward)

## Game Mechanics

- The ball starts on the left paddle
- Press A or D to rotate the paddles and launch the ball
- Hit green targets to score points
- The ball bounces off walls and paddles with realistic physics
- Special ice zone creates a blue trail effect when the ball passes through

## Technical Details

### Scripts

- `PinballGameSetup.cs`: Main setup script that creates all game objects
- `BallController.cs`: Controls ball movement, speed limits, and collision handling
- `PaddleController.cs`: Handles paddle rotation and ball collision
- `ScoreManager.cs`: Manages game score
- `ParticleTrigger.cs`: Triggers particle effects on target hits
- `PhysicsZone.cs`: Applies different physics materials to zones
- `UIManager.cs`: Dynamically creates and manages UI elements

### Physics

- Ball is constrained to XZ plane (no Y-axis movement)
- Paddles rotate around their edge points
- Collision detection uses Unity's physics engine
- Special handling for 90-degree collisions to prevent infinite loops

## Requirements

- Unity 2021.3 or later
- No additional packages required (uses built-in Unity features)

## Setup

1. Clone this repository
2. Open the project in Unity
3. The game will automatically set up when you press Play
4. All game objects, UI, and physics are created via scripts

## License

This project is open source and available for educational purposes.

