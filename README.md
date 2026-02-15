# Knock Knock: Who's There? (Assignment 2)

Members: Nurali, Omar, Areeb
## 1. Players
- 2 human players (1v1):
  - **Thief (Attacker)**
  - **Defender**

## 2. Objective (Win Condition)
- Be the first player to complete all 10 rooms in order and reach the **Vault** trigger.

## 3. Core Rules
- Rooms must be completed in sequence (no skipping).
- Each room has 1 question and 4 answer doors.
- Correct door: `+1` score and advance.
- Wrong normal door: `-2` score.
- If room is a trap room and player chooses a wrong door: `-5` score and teleport to start checkpoint.

## 4. Setup Phase
- Thief selects exactly 3 trap rooms.
- Defender selects one question set (A, B, or C).
- Both players confirm ready.
- Server starts race after valid setup from both players.

## 5. Procedures / Flow
1. Host starts game and shares Relay join code.
2. Client joins.
3. Setup phase (trap rooms + question set + ready).
4. Race phase through 10 rooms.
5. First valid player entering vault wins.

## 6. Resources / State
- Score per player.
- Current room progress per player.
- Selected question set (A/B/C).
- Trap room set (chosen by Thief).
- Checkpoints (start + room checkpoints).

## 7. Conflict / Challenge
- Both players race on the same question path.
- Thief creates strategic pressure via trap rooms.
- Defender must optimize correct choices and avoid penalties.

## 8. Boundaries
- Multiplayer over NGO + Relay + UTP.
- Server-authoritative logic for validation and scoring.
- Door triggers send requests; server decides outcomes.

## 9. Controls (Current Build)
- Move: `W A S D`
- Jump: `Space`
- Setup keys:
  - Trap rooms: `1..9` and `0` (rooms 0..9)
  - Question set: `Z` (A), `X` (B), `C` (C)
  - Ready: `R`

## 10. Tech
- Unity
- Netcode for GameObjects (NGO)
- Unity Relay
- Unity Transport (UTP)
