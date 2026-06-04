# Echo Nomad — Part 1

A Unity 3D third-person exploration prototype built with Unity 2022.3 LTS and the Universal Render Pipeline (URP).

## Overview

Echo Nomad Pt. 1 is the first development milestone of an open-world exploration game. This build establishes the core environment and player movement foundations: a hand-painted terrain, vegetation system, rock formations, and a fully rigged third-person character controller.

## Features

- Third-person character movement using Unity's Starter Assets input system
- Terrain sculpted and painted with Unity Terrain Tools and Polybrush
- Vegetation including realistic trees (Tree9, Realistic Tree, Conifers via CTI wind shaders)
- PBR rock cliff assets with multiple material variants (cliffs, moss, snow, cracked)
- Grass and ground terrain layers with handpainted textures
- Cinemachine camera setup
- Universal Render Pipeline (URP) with post-processing

## Tech Stack

| Tool | Version |
|------|---------|
| Unity Editor | 2022.3.60f1 |
| Render Pipeline | URP 14.0.12 |
| Input System | 1.11.2 |
| Cinemachine | 2.10.3 |
| Terrain Tools | 5.0.6 |
| Polybrush | 1.1.8 |
| TextMeshPro | 3.0.7 |
| Visual Scripting | 1.9.4 |

## Getting Started

1. Clone the repository
2. Open the project in **Unity 2022.3.x** (LTS)
3. Open `Assets/Scenes/SampleScene.unity`
4. Press Play

> Unity will prompt to install any missing packages automatically via the Package Manager.

## Project Structure

```
Assets/
├── Scenes/              # Main scene (SampleScene)
├── StarterAssets/       # Third-person controller, input system, mobile support
├── character/           # Kachujin G Rosales character model & textures
├── trees/               # Tree9 prefabs and materials
├── Realistic Tree/      # Realistic tree asset pack
├── Forst/               # CTI wind shaders & Conifers [BOTD]
├── PBR_Rock_Cliffs_Pack/ # Rock cliff 3D models and PBR materials
├── GrassFlowers/        # Grass/flower assets with post-processing profile
└── Polybrush Data/      # Brush and palette settings for terrain painting
```

## Controls

| Action | Key |
|--------|-----|
| Move | WASD / Left Stick |
| Camera | Mouse / Right Stick |
| Jump | Space / South Button |
| Sprint | Shift / Left Stick Click |

## Third-Party Assets

- **Unity Starter Assets** — Unity Companion License
- **Realistic Tree Pack** — see `Assets/Realistic Tree/Documentation.pdf`
- **PBR Rock Cliffs Pack** — see `Assets/PBR_Rock_Cliffs_Pack/Read_me.txt`
- **Tree9** — included tree asset pack
- **Forst / CTI** — Conifers with CTI wind runtime components
