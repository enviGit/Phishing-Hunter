<h1 align="center">Phishing Hunter</h1>
<p align="center">
  <b>A cyber awareness game: hunt phishing, master security, and climb the corporate ladder.</b>
  <br>
  <img src="https://img.shields.io/badge/license-All%20rights%20reserved-red.svg" alt="All rights reserved">
  <a href="https://unity.com/"><img src="https://img.shields.io/badge/engine-Unity-222c37.svg"></a>
</p>

---

## 📂 About

**Phishing Hunter** is an educational game built with Unity, set in a simulated office environment. As a cybersecurity analyst, your core challenges are to identify suspicious emails (phishing), solve real-world security quizzes, and progress your virtual career by learning the fundamentals of digital hygiene.

The project is designed to promote cyber awareness and foster safe online habits in an engaging, interactive way.

---

## 👥 Team

**Phishing Hunter** was developed by a two-person team, combining technical engineering with creative design:

* **Paweł Trojański ([@enviGit](https://github.com/enviGit))** – **Lead Developer**
    * System Architecture & Core Logic (C#)
    * Implementation of OS simulation tools (Mail, Terminal, Paint)
    * Save/Load System & Data Serialization
    * Debugging & Preliminary Testing
* **Dawid Mucha ([@BzykuuDM](https://github.com/BzykuuDM))** – **UI/UX Designer**
    * Visual Identity & Interface Design
    * Level Design & Environmental Assets
    * Quality Assurance (QA) & Playtesting

---

## 🎮 Core Features

- **Simulated Operating System:** Multiple functional applications (Mail, Quiz, Paint, Music Player, Notes, Terminal, etc.).
- **Phishing Detection Gameplay:** Analyze headers and content to tag emails as safe or phishing; review realistic attachments with zoom functionality.
- **Robust Save System:** Encrypted save/load functionality with auto-save support ensuring data persistence (JSON-based).
- **Interactive Security Quizzes:** Single- and multiple-choice questions with instant educational feedback.
- **Career Progression:** Experience levels, professional titles (Intern to Expert), achievements, and adaptive difficulty.
- **Localization:** Support for multiple languages (English, Polish, etc.) via external JSON files.
- **High Performance:** Optimized custom shaders and DOTween animations for smooth 60+ FPS UI transitions.

---

## ✨ Recent & Notable Improvements

- **Optimization:** Refactored UI rendering pipeline and shaders to boost framerate; optimized DOTween sequences for lag-free menu navigation.
- **Data Persistence:** Fully implemented `IDataPersistence` interface for secure game state saving (player progress, unlocked achievements, flagged emails).
- **Dynamic Content:** Incremental incoming emails generation instead of static lists.
- **Attachment System:** PNG preview + zoom implemented (foundation laid for PDF/EXE simulation).
- **Multilingual Support:** Smooth font support for complex scripts (Chinese, Korean, Russian, Polish) with runtime language switching.
- **Audio/Visual Feedback:** Enhanced sound effects and responsive animated button states (hover/click).

---

---

## 🚧 Roadmap

- Expanded attachment simulations (.exe warnings, .pdf viewer)
- "Cyberpedia" – collectible cards with security facts
- Focus switching improvements (taskbar logic)
- Cinematic cutscenes and narrative elements
- Office simulation expansion (NPCs, day/night cycle)
- Advanced UI scaling for 4K/Ultrawide monitors
- Further resource management optimizations (Addressables)

---

## 📜 License

**All rights reserved.** Redistribution, modification, or commercial use of any part of this project is **prohibited** without explicit, written permission from the authors.

---
