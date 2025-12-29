# Rocket Game Technical Overview

## 1. Project Architecture
The project is a level-based physics game built in **Unity** using **C#**. It utilizes the standard Unity lifecycle (`Start`, `Update`, `FixedUpdate`) and built-in Physics engine (`Rigidbody`, `Collision`).

### Core Components
*   **Player Controller (`Movement.cs`):** Handles physics-based movement and rotation.
*   **State Manager (`CollisionHandler.cs`):** Manages game state (Playing, Crashing, Ascending) and scene transitions.
*   **Environmental Logic (`Oscillator.cs`):** Controls dynamic obstacles.
*   **System (`QuitApplication.cs`):** Handles application lifecycle.

## 2. Core Mechanics

### Physics & Controls
The player controls a rocket with a **Rigidbody** component.
*   **Thrust:** Applied in `FixedUpdate` using `rb.AddRelativeForce(Vector3.up)` to ensure consistent physics simulation independent of frame rate.
*   **Rotation:** Applied in `Update` using `Transform.Rotate(Vector3.forward)`.
    *   *Constraint Logic:* The Rigidbody's rotation is frozen (`rb.freezeRotation = true`) while the player is manually rotating to prevent the physics engine from interfering, and unfrozen immediately after.
*   **Feedback:** Particle systems and Audio clips trigger on specific input states (Thrusting, Turning Left/Right).

### Collision System
The `CollisionHandler` uses `OnCollisionEnter` to detect interactions based on **Tags**:
*   **"Friendly":** Safe zones (e.g., Launch pad). No action.
*   **"Finish":** Landing pad. Triggers "Success Sequence" (disable controls -> play sound/particles -> load next level).
*   **"Default" (Untagged):** Obstacles/Terrain. Triggers "Crash Sequence" (disable controls -> play crash sound/particles -> reload current level).

**Debug Tools:**
*   `L`: Instantly load the next level.
*   `C`: Toggle collision detection (God Mode).

### Dynamic Environment
Obstacles use the `Oscillator` script to move back and forth.
*   **Logic:** Uses a Sine wave formula (`Mathf.Sin`) based on `Time.time` to calculate a `movementFactor` (0 to 1).
*   **Implementation:** Interpolates between a `startingPosition` and a target offset (`movementVector`) to create smooth, looping movement.

## 3. Game Loop
1.  **Start:** Rocket spawns on a "Friendly" pad.
2.  **Play:** Player navigates obstacles using thrust and rotation.
3.  **End Condition:**
    *   **Win:** Touch "Finish" pad -> Wait Delay -> Load Next Scene.
    *   **Loss:** Touch Obstacle -> Wait Delay -> Reload Current Scene.
4.  **Progression:** Levels are loaded by Build Index. If the last level is completed, the game loops back to the first level.

---

# AI Generation Prompt

**Prompt:**

> "Create a 2.5D physics-based skill game in Unity using C#. The game involves piloting a rocket ship from a starting landing pad to a destination landing pad while avoiding static and moving obstacles.
>
> **Core Requirements:**
>
> 1.  **Player Controller:**
>     *   Implement a script that uses a `Rigidbody` for movement.
>     *   **Thrust:** Apply relative physics force upwards when the user presses Space.
>     *   **Rotation:** Directly modify the Transform rotation when pressing A or D (Left/Right). Ensure physics rotation is temporarily disabled during manual input to keep controls snappy.
>     *   Include audio and particle effects that toggle on/off based on input state.
>
> 2.  **Game State & Collisions:**
>     *   Create a collision handler that detects object tags.
>     *   **Win Condition:** colliding with an object tagged "Finish" should disable controls, play a success effect, and load the next level after a short delay.
>     *   **Loss Condition:** colliding with anything else (terrain, obstacles) should disable controls, play an explosion effect, and reload the current level.
>     *   **Safe Zones:** Collisions with "Friendly" objects (like the spawn pad) should be ignored.
>     *   *Debug Features:* Add keys to instantly load the next level and to toggle collision detection on/off.
>
> 3.  **Environment:**
>     *   Create a script for moving obstacles that oscillate between two points using a Sine wave function for smooth, continuous loops.
>
> 4.  **Architecture:**
>     *   Use `FixedUpdate` for all physics force applications.
>     *   Use `Update` for input detection and visual updates.
>     *   Ensure the scene management logic handles looping back to the first level after the final level is completed."
