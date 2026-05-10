# [Critters](https://github.com/Benjamin-Scherrer/Critters)
## Abstract
Move animated geometrical bodies with playful interactions and combine them into imaginative creatures. 
Individuals coalesce into dynamic unity. 
This playful interaction is based on the modular design of Sophie Taeuber-Arp's puppets, 
which inspire through their clear shape language and flexible combinability.

Created by Gierer Damian, Häfliger Nadja, Scherrer Benjamin, Staub Michael

## Context
Critters, previously called Locketry, is a digital exhibition piece made at [ZHdK](https://www.zhdk.ch/en/zurich-university-of-the-arts-1) as part of the minor [Digital Play](https://www.zhdk.ch/en/degree-programmes/digital-play-10126).
Critters was developed in cooperation with the [Museum of Design Zurich](https://museum-gestaltung.ch/en) for the exhibition [Museum of the Future](https://www.emuseum.ch/exhibitions/1974/museum-of-the-future;jsessionid=914C6D83638B904FA966B7D975D71AEF?ctx=240f2c0af246d43e21d56fa5849efaf736fadd33&idx=6).

## Overview
The game is displayed in a vertical format ideal for exhibition and controlled with a touchscreen connected through [Touch Designer](https://derivative.ca/). The system regularly resets to prevent bricking within an exhibition context. Each loop is randomized, with simple interaction rules driving the game elements.

<img width="510" height="899" alt="Finished_Gameplay_Screenshot" src="https://github.com/user-attachments/assets/70a75556-efb3-449a-a5d8-6c186272123d" />

Each geometrical body, a critter, is self propelled.
A critter will lock onto another critter if they're close enough to each other.

https://github.com/user-attachments/assets/4dba9232-943a-42bb-8b46-64532b839ec0

Once enough critters form an amalgam together its animations become more complex and the amalgam gains a level of verticality.
If an amalgam has existed for a set amount of time without changing, it runs out of its lifetime and disasembles into its base critters.

https://github.com/user-attachments/assets/808ed197-8a0e-4d2d-ae7d-f1e61f3587af

## Design
Each critter has their own unique style of movement pattern. 
The visitors can interact with the critters by dragging them across the screen. 
The interaction device is a touchscreen that's mapped to the display screen.
The touchscreen is mostly dark, with minimal interaction feedback being displayed upon touch.

<img width="1920" height="1080" alt="Skizze_Critters" src="https://github.com/user-attachments/assets/c8b5dc03-1fb1-4ae6-b2eb-ae8416837e54" />

Each shape of a critter is inspired by a segment of [Sophie Taeuber-Arp](https://sophietaeuberarp.org/english/)'s marionettes. 
The color palette is chosen to be harmonious in any combination.

<img width="1920" height="1080" alt="Skizze_Critters2" src="https://github.com/user-attachments/assets/65fb7168-bdcc-475f-a763-33298d46d2f6" />

## Exhibition
During the exhibition [Museum of the Future](https://www.emuseum.ch/exhibitions/1974/museum-of-the-future;jsessionid=914C6D83638B904FA966B7D975D71AEF?ctx=240f2c0af246d43e21d56fa5849efaf736fadd33&idx=6) within the main building of the [Museum of Design Zurich](https://museum-gestaltung.ch/en) Critters was displayed from the 29. August 2025 until 1. February 2026.
By running the system on a local computer hidden within the exhibition pedestal, the project was self-contained and easy to reboot.
The project would start automatically before the opening hours of the exhibition and shut down after the closing hours.
During the day the game reset itself periodically.
If the game crashed an overseeing script would restart the game application.
Inside the exhibition pedestal custom buttons allowed museum staff to manually restart the application and the computer without interacting with the PC directly.

## Technical Details
Critters was developed on [Unity](https://unity.com/) [2022.3.51f1](https://unity.com/releases/editor/whats-new/2022.3.51f1).
Core packages were the [Cinemachine](https://docs.unity3d.com/Packages/com.unity.cinemachine@6.6/manual/index.html) package by Unity and the [Unity Essentials](https://github.com/d3tonat0r/unitypackage-essentials) package by [Gianmarco Di Vincenzo](https://www.gmdv.ch/).
[FMOD](https://www.fmod.com/) is used for the dynamic music and sfx.
The touchscreen is implemented on a tablet with [Touch Designer](https://derivative.ca/).

## Credits

Michael Staub: Producer, Programmer (Prototyping, Exhibition Utility) \
Nadja Hälfiger: Visual Lead, 3D Designer, Interaction Design  \
Damian Grierer: Programmer (Input System, Critter Spawning, Computer Graphics) \
Benjamin Scherrer: Programmer (Critter Movement, Combination System)

## Acknowledgements

Thanks to [Maike Thies](https://intern.zhdk.ch/?person/detail&id=178918), [Florian Faller](https://intern.zhdk.ch/?person/detail&id=111717), [Stefan Kraft](https://intern.zhdk.ch/?person/detail&id=221899), and [Johannes Reck](https://intern.zhdk.ch/?person/detail&id=237558) for their continued support in administrative matters, physical implementations, and their professional feedback.
