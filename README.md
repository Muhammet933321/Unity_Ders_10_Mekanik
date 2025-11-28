# Unity Mechanics – Car & Dart

This repository contains two independent gameplay mechanics created in Unity:

## 🚗 Car Mechanic
The car’s behavior is controlled through **Scriptable Objects**, allowing easy customization:
- Torque
- Power
- Maximum speed
- Acceleration & braking settings

The controller script reads these values from the Scriptable Object and applies them to the car’s physics, making it easy to create different vehicle profiles without modifying code.

## 🎯 Dart Mechanic
A dart is thrown toward a target board, and points are awarded based on the hit:
- Score based on hit area
- Rigidbody-based physics
- Simple scoring system

## 📁 Project Structure
- `Car` folder → Car mechanic scripts, prefabs, and scene  
- `Dart` folder → Dart mechanic scripts, prefabs, and scene  
- Scriptable Objects store car stat presets  
- Each mechanic runs in its own scene  

## ▶️ How to Use
1. Clone the repository.  
2. Open it in Unity.  
3. Run the Car or Dart scene to test the mechanics.  

