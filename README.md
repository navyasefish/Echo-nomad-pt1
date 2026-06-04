# Echo Nomad

A 3D exploration game where the environment itself becomes a source of music. Developed in Unity 3D as a B.Tech Computer Science project at the University of Petroleum & Energy Studies (UPES), Dehradun.

**Developers:** Navya Nandini, Harshika Bilwal
**Guide:** Mr. Priyabrata
**Institution:** School of Computer Science, UPES — September 2025

---

## About

Echo Nomad is a calming, single-player exploration game that blends open-world traversal with interactive sound mechanics. Players take on the role of the Echo Nomad — a wandering explorer guided not by combat or conquest, but by the pursuit of music.

Nearly every object in the environment (stones, flowers, trees, water, wildlife) produces a unique sound when interacted with. By experimenting and combining these natural sounds into melodies, players progress through the world. Each biome is guarded by a **Guardian of Sound** who challenges the player to recreate a melody using environmental objects — unlocking new areas upon success.

The current prototype covers the **Forest biome**, serving as the foundation for future expansions (Desert, Mountains, and beyond).

> Inspired by: *Journey*, *Abzû*, *Flower*, *Sound Shapes*

---

## Core Gameplay Mechanics

- **Exploration** — Traverse natural biomes guided by visual and auditory cues. No combat, no timers.
- **Environmental Interaction** — Interact with stones, flowers, water, trees, bamboo, and wildlife to produce unique sounds.
- **Sound Combination** — Layer and combine natural sounds to form melodies. Example combinations:
  - Stones + Water → splash percussion
  - Wind + Bamboo → flute-like melody
  - Stick + Hollow Trunk → drumbeat
  - Birds + Waterfall → layered natural harmony
- **Melody Challenges** — Guardians of sound present 3–6 note melodies that must be recreated using environmental objects to unlock new areas.

---

## Features

- Third-person character controller with Cinemachine camera
- Hand-sculpted forest terrain using Unity Terrain Tools and Polybrush
- Interactive sound objects: stones, bell-shaped flowers, bamboo, hollow trunks, rivers, waterfalls
- Wildlife with proximity-reactive behavior (birds, woodpeckers, insects)
- Dynamic audio layering via Unity Audio Mixer — ambient sounds shift in intensity based on player proximity
- Melody challenge system that checks pitch, rhythm, and sequence, while allowing creative flexibility
- Minimal HUD with contextual interaction prompts — no health bars or failure penalties
- Accessibility support: visual rhythm cues, melody subtitles, adjustable audio settings
- URP rendering with dynamic shadows, reflective water, and particle effects (drifting spores, foliage sway)

---

## Genre & Platform

| | |
|---|---|
| **Genre** | 3D Exploration / Musical Puzzle |
| **Platform** | PC (Windows) |
| **Play-style** | Single-player, open-ended |
| **Engine** | Unity 2022.3 LTS (URP) |

---

## Getting Started

1. Clone the repository
2. Open the project in **Unity 2022.3.x** (LTS)
3. Open `Assets/Scenes/SampleScene.unity`
4. Press Play

> Unity will resolve any missing packages automatically via the Package Manager.

---

## Controls

| Action | Keyboard / Mouse | Controller |
|--------|-----------------|------------|
| Move | WASD | Left Stick |
| Camera | Mouse | Right Stick |
| Jump | Space | South Button |
| Sprint | Shift | Left Stick Click |
| Interact | E | East Button |

---

## Tech Stack

| Tool | Version / Purpose |
|------|------------------|
| Unity Editor | 2022.3.60f1 |
| Render Pipeline | URP 14.0.12 |
| Language | C# |
| Input System | Unity Input System 1.11.2 |
| Cinemachine | 2.10.3 |
| Terrain Tools | 5.0.6 |
| Polybrush | 1.1.8 |
| TextMeshPro | 3.0.7 |
| 3D Modeling | Blender |
| Concept Art / Textures | Adobe Illustrator, Krita |
| Sound Recording & Editing | Audacity |
| Sound Composition | FL Studio, Ableton Live |
| Version Control | Git |

---

## Project Structure

```
Assets/
├── Scenes/               # Main scene (SampleScene)
├── StarterAssets/        # Third-person controller, input system, mobile support
├── character/            # Kachujin G Rosales character model & textures
├── trees/                # Tree9 prefabs and materials
├── Realistic Tree/       # Realistic tree asset pack
├── Forst/                # CTI wind shaders & Conifers [BOTD]
├── PBR_Rock_Cliffs_Pack/ # Rock cliff 3D models and PBR materials
├── GrassFlowers/         # Grass/flower assets with post-processing profile
└── Polybrush Data/       # Brush and palette settings for terrain painting
```

---

## System Requirements

| | Minimum | Recommended |
|---|---|---|
| RAM | 4 GB | 8 GB |
| CPU | 2 GHz | 3 GHz |
| GPU | Integrated | Dedicated GPU |
| Storage | HDD | SSD |

---

## Roadmap

- **Desert biome** — shifting sands, hollow winds, resonant rocks
- Longer, more complex melody challenges with layered harmonies
- Additional biomes: mountains, caves, surreal otherworldly realms
- Performance optimization for mid-range hardware
- Expanded accessibility and community playtesting

---

## Third-Party Assets

- **Unity Starter Assets** — Unity Companion License (`Assets/StarterAssets/license.txt`)
- **Realistic Tree Pack** — see `Assets/Realistic Tree/Documentation.pdf`
- **PBR Rock Cliffs Pack** — see `Assets/PBR_Rock_Cliffs_Pack/Read_me.txt`
- **Tree9** — included tree asset pack
- **Forst / CTI** — Conifers with CTI wind runtime components
