# Physics-Based Arcade Car Controller

A Unity-based arcade car controller built to explore vehicle physics, suspension simulation, and modular gameplay architecture.

## Overview

This project was created as a learning exercise to gain a deeper understanding of Unity's physics system and how vehicle behavior can be achieved through force-based simulation rather than transform manipulation.

The controller focuses on creating a responsive arcade driving experience while maintaining believable vehicle movement through a custom suspension system, steering mechanics, and motor simulation.

The project follows a modular architecture where vehicle responsibilities are separated into independent systems, making the controller easier to maintain, extend, and tune.

---

## Features

### Suspension System

* Raycast-based wheel/suspension detection
* Spring-damper simulation
* Configurable spring stiffness
* Configurable damper stiffness
* Adjustable suspension travel
* Force application at wheel positions

### Steering System

* Smooth steering interpolation
* Torque-based vehicle turning
* Sideways grip correction
* Steering stabilization
* Drift-friendly handling through adjustable grip values
* Reverse steering correction

### Motor System

* Physics-driven acceleration
* Forward and reverse movement
* Braking system
* Rolling resistance simulation
* Handbrake functionality
* Controlled drifting behavior

### Architecture

* Modular design
* Separation of concerns
* Constructor-based dependency injection approach
* Independent systems for:

  * Suspension
  * Steering
  * Motor Control

---

## Project Structure

```text
CarController
│
├── CarSuspensionModule
│   ├── Ground Detection
│   ├── Spring Calculation
│   └── Damper Calculation
│
├── CarSteeringModule
│   ├── Steering Input
│   ├── Grip Correction
│   └── Turning Torque
│
└── CarMotorModule
    ├── Acceleration
    ├── Braking
    ├── Rolling Resistance
    └── Handbrake Logic
```

---

## Suspension System

The suspension system uses raycasts from wheel positions to detect the ground surface.

When a wheel is grounded:

1. Current suspension compression is calculated.
2. Spring force is generated based on compression.
3. Damping force is calculated using wheel velocity.
4. The resulting force is applied to the rigidbody at the suspension point.

This creates a more responsive and believable vehicle feel compared to directly moving the vehicle transform.

---

## Steering System

Vehicle steering is achieved using Rigidbody torque rather than directly rotating the vehicle.

The system includes:

* Smoothed steering input
* Speed-based steering response
* Sideways velocity correction
* Adjustable grip values
* Yaw damping for stability

These systems work together to create an arcade-style driving experience while still being physics-driven.

---

## Motor System

The motor module handles:

* Acceleration
* Reverse movement
* Braking
* Rolling resistance
* Handbrake-assisted drifting

All movement forces are applied relative to the vehicle's forward direction, ensuring that propulsion remains consistent with the current heading of the car.

---

## What I Learned

Through this project I gained practical experience with:

* Unity Rigidbody Physics
* Force-based movement systems
* Raycast suspension simulation
* Spring-damper mechanics
* Vehicle tuning and handling
* Dependency Injection concepts
* Modular architecture design
* Separation of responsibilities
* Debugging and balancing physics systems

One of the most valuable lessons from this project was understanding how parameters such as spring stiffness, damping, grip, torque, and resistance directly influence vehicle feel and player experience.

---

## Inspiration & References

The suspension implementation was heavily inspired by concepts demonstrated by Ash Dev's vehicle physics content. His explanations helped me better understand spring-damper systems and vehicle suspension fundamentals while building and experimenting with my own implementation.

---

## Future Improvements

* Wheel visual suspension animation
* Different surface friction types
* ABS braking simulation
* Traction control
* Gearbox system
* Engine RPM simulation
* Four-wheel drive support
* AI-controlled vehicles
* Multiplayer compatibility

---

## Built With

* Unity
* C#
* Rigidbody Physics
* Raycasting
* Custom Vehicle Systems
