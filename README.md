# Unity-Projektbausteine – Top-Down Pixel-Art Action-RPG

Kleine, einzeln nutzbare C#-Bausteine für ein Top-Down-Spiel, das Elemente aus
**Diablo 2** (Beute, Werte, Level), **Hollow Knight** (Kampfgefühl, Dash, Rastbänke, verlorenes Geld),
**Stardew Valley** (Farming, Tageszeit, Werkzeuge) und **Zelda** (Truhen, Schlüssel, Räume, Herzen) mischt.

Jedes Skript erledigt **genau eine Aufgabe**. Die Bausteine kommunizieren über Events
(z.B. `Health.Died`) und Interfaces (`IDamageable`, `IInteractable`, `IToolTarget`),
statt sich direkt zu kennen. Dadurch kannst du sie frei kombinieren, austauschen oder weglassen.

## Voraussetzungen

| Was | Warum |
|---|---|
| **Unity 6** (6000.x) | Nutzt `Rigidbody2D.linearVelocity`. In Unity 2022/2023 stattdessen `velocity` verwenden. |
| Paket **Input System** | Eingaben (`PlayerInputReader`, `Hotbar`). Player Settings → *Active Input Handling* = *Input System Package* oder *Both*. |
| **TextMeshPro** | UI-Texte (in Unity 6 im uGUI-Paket enthalten). |
| Tag **Player** | Gegner, Kamera, UI und Pickups finden den Spieler über diesen Tag. |

Alle Skripte liegen im Namespace `Bausteine` unter `Assets/Scripts/Bausteine/`.

## Ordnerübersicht

### Core – Grundlagen für Kampf & Schaden
| Skript | Aufgabe |
|---|---|
| `DamageInfo` | Datenpaket eines Treffers (Menge, Quelle, Rückstoß, Krit). |
| `IDamageable` | Interface für alles, was Schaden nehmen kann. |
| `TeamMember` | Fraktion (Spieler / Gegner / Neutral) – verhindert Friendly Fire. |
| `Health` | Lebenspunkte mit Events (`Damaged`, `Healed`, `Changed`, `Died`, `Revived`), Rüstung optional. |
| `Hitbox` | Trigger, der beim Aktivieren einmal pro Ziel Schaden austeilt. |
| `ContactDamage` | Schaden bei Körperkontakt (Gegner). |
| `Knockback` | Rückstoß bei Treffern, sperrt kurz die Steuerung. |
| `Invincibility` | I-Frames + Blinken nach einem Treffer. |
| `DamageFlash` | Sprite blitzt bei Treffern kurz auf. |
| `HitStop` | Mini-Zeitstopp bei Treffern für wuchtiges Kampfgefühl. |
| `HitFeedback` | Verbindet Hitbox mit HitStop, CameraShake und Treffer-Effekt. |
| `DestroyOnDeath` | Entfernt Objekte nach dem Tod (Gegner, Krüge). |
| `DamageCalculator` | Zentrale Schadensformel (Stats, Krit, Rüstung). |

### Player – Steuerung
| Skript | Aufgabe |
|---|---|
| `PlayerInputReader` | Liest Tastatur/Maus/Gamepad; einzige Stelle für Eingaben. |
| `FacingDirection` | Blickrichtung (auch im Stand), 4-Wege-Rundung für Sprites. |
| `PlayerMovement` | 8-Wege-Bewegung mit Beschleunigung. |
| `PlayerDash` | Dash/Ausweichrolle mit I-Frames, per Fähigkeit freischaltbar. |
| `PlayerMeleeAttack` | Schwerthieb in Blickrichtung (Ausholen → Treffen → Erholen). |
| `PlayerInteractor` | Findet Truhen/Schilder/NPCs vor dem Spieler und interagiert. |
| `PlayerAnimatorBridge` | Überträgt Zustand an den Animator. |
| `PlayerControlOnDeath` | Sperrt die Steuerung, solange der Spieler tot ist. |

### Stats – Werte & Level (Diablo)
| Skript | Aufgabe |
|---|---|
| `StatType` | Liste aller Werte (Schaden, Rüstung, Krit, Magic Find …). |
| `StatModifier` / `StatBonus` | Flat / Prozent-Boni, im Inspector einstellbar. |
| `Stat` | Ein Wert mit Basis + Modifikatoren. |
| `CharacterStats` | Alle Werte eines Charakters. |
| `StatsToHealthLink` | Hält `Health.Max` synchron mit dem Wert MaxHealth. |
| `ExperienceSystem` | EP, Level-Ups, Stat-Punkte verteilen. |
| `ExperienceReward` | Gegner gibt dem Töter EP. |

### Items – Gegenstände, Inventar, Beute
| Skript | Aufgabe |
|---|---|
| `ItemData` | Gegenstand als ScriptableObject. |
| `EquipmentData` | Ausrüstung mit Slot und Boni. |
| `ItemRarity` | Seltenheit + Diablo-Farben. |
| `ItemStack` | „Gegenstand × Anzahl“. |
| `Inventory` | Feste Slots, automatisches Stapeln. |
| `Equipment` | Anlegen/Ablegen, überträgt Boni auf Stats. |
| `Wallet` | Geld. |
| `LootTable` | Gewichtete Beutetabelle mit Magic Find & Gold. |
| `LootDropper` | Lässt beim Tod Beute fallen. |
| `PickupSpawner` | Zentrale Stelle zum Erzeugen von Beute in der Welt. |
| `ItemPickup` / `CurrencyPickup` | Einsammelbare Gegenstände/Münzen. |
| `PickupMagnet` | Zieht Beute zum Spieler. |
| `ItemDatabase` | ID → ItemData (für Spielstände). |

### Enemies – Gegner
| Skript | Aufgabe |
|---|---|
| `TargetDetector` | Sieht den Spieler (Radius + Sichtlinie), reagiert auf Treffer. |
| `EnemyBrain` | Zustandsautomat: Idle → Wander → Chase → Attack → Return. |
| `EnemyAttack` | Basisklasse für Angriffe (Reichweite, Cooldown). |
| `EnemyMeleeAttack` | Nahkampf mit Ankündigung und optionalem Vorstoß. |
| `EnemyRangedAttack` | Fernkampf, auch als Projektilfächer. |
| `Projectile` | Fliegendes Geschoss. |
| `EnemySpawner` | Erzeugt Gegner, optional mit Respawn. |

### World – Welt & Erkundung (Zelda / Hollow Knight)
| Skript | Aufgabe |
|---|---|
| `IInteractable` | Interface für alles, womit man interagieren kann. |
| `Chest` | Truhe mit festem Inhalt und/oder LootTable. |
| `Sign` | Schild / einfacher NPC mit Text. |
| `LockedDoor` | Tür, die einen Schlüssel braucht. |
| `WorldFlags` | Globales Gedächtnis („Truhe X offen“). |
| `PlayerAbilities` / `AbilityPickup` / `AbilityGate` | Fähigkeiten freischalten und damit Wege öffnen. |
| `Checkpoint` | Rastbank: heilen, Wiedereinstiegspunkt, speichern. |
| `PlayerRespawn` | Wiederauferstehen am Checkpoint. |
| `CurrencyLossOnDeath` / `LostCurrencyMarker` | Geld bleibt am Sterbeort liegen (Hollow Knight). |
| `SceneDoor` / `SpawnPoint` | Szenenwechsel mit Ankunftspunkt. |

### Farming – Landwirtschaft & Zeit (Stardew Valley)
| Skript | Aufgabe |
|---|---|
| `GameClock` | Uhrzeit, Tage, Jahreszeiten, Jahre. |
| `DayNightOverlay` | Tönt den Bildschirm passend zur Tageszeit. |
| `ToolData` / `SeedData` / `CropData` | Werkzeuge, Saatgut, Pflanzenarten. |
| `IToolTarget` | Interface für alles, worauf man Werkzeuge anwendet. |
| `Hotbar` | Schnellleiste (1–9, Mausrad, Schultertasten). |
| `ToolUser` | Benutzt den gehaltenen Gegenstand auf das Ziel vor dem Spieler. |
| `Stamina` | Ausdauer für Werkzeuge. |
| `FarmPlot` | Ein Feld: hacken, säen, gießen, wachsen, ernten. |
| `ResourceNode` | Baum/Stein, abbaubar mit passendem Werkzeug. |
| `Bed` | Schlafen → nächster Tag, auffüllen, speichern. |

### Camera
| Skript | Aufgabe |
|---|---|
| `CameraFollow` | Weiches Folgen, Raumgrenzen, Pixel-Raster. |
| `CameraShake` | Bildschirmwackeln. |
| `CameraRoom` | Raum mit eigenen Kameragrenzen. |

### UI
| Skript | Aufgabe |
|---|---|
| `HeartDisplay` | Leben als Herzen. |
| `ResourceBar` | Füllleiste für Leben, Ausdauer oder EP. |
| `WalletText` / `ClockText` | Geld bzw. Uhrzeit als Text. |
| `InteractPrompt` | „[E] Öffnen“-Hinweis. |
| `DialogueBox` | Textbox mit Schreibmaschinen-Effekt. |
| `DamagePopup` / `DamagePopupSpawner` | Schwebende Schadenszahlen. |

### Save
| Skript | Aufgabe |
|---|---|
| `SaveSystem` | JSON speichern/laden (absturzsicher über Temp-Datei). |
| `PlayerSaveData` | Alle gespeicherten Daten. |
| `PlayerSaveController` | Sammelt den Zustand aller Bausteine und stellt ihn wieder her. |

---

## Bauanleitungen

### Spieler-Prefab
```
Player  (Tag: Player)
│  Rigidbody2D (Dynamic, Gravity 0)        CapsuleCollider2D
│  TeamMember (Player)                     Health   Knockback   Invincibility   DamageFlash
│  PlayerInputReader   FacingDirection (Input zuweisen)   PlayerMovement   PlayerDash
│  PlayerMeleeAttack   PlayerInteractor    PlayerControlOnDeath   PlayerRespawn
│  CharacterStats   StatsToHealthLink   ExperienceSystem
│  Inventory   Equipment   Wallet   Hotbar   ToolUser   Stamina
│  PlayerAbilities   CurrencyLossOnDeath   PlayerSaveController
├─ Sprite         (SpriteRenderer + Animator + PlayerAnimatorBridge)
└─ AttackPivot    (leer, Position 0,0)
    └─ Hitbox     (Position x = 0.6; BoxCollider2D Trigger + Hitbox + HitFeedback)
```

### Gegner-Prefab
```
Slime
│  Rigidbody2D (Dynamic, Gravity 0)   CircleCollider2D
│  TeamMember (Enemy)   Health   Knockback   DamageFlash   DestroyOnDeath
│  TargetDetector   EnemyBrain   EnemyMeleeAttack (oder EnemyRangedAttack)
│  ContactDamage   LootDropper   ExperienceReward   DamagePopupSpawner
│  (optional) CharacterStats   FacingDirection
└─ AttackPivot → Hitbox   (wie beim Spieler)
```

### Zerstörbarer Krug / Busch (Zelda)
`SpriteRenderer` + `BoxCollider2D` + `TeamMember (Neutral)` + `Health (1)` + `LootDropper` + `DestroyOnDeath`

### Beute-Prefabs (für den `PickupSpawner`)
`SpriteRenderer` + `CircleCollider2D (Trigger)` + `Rigidbody2D (Gravity 0, Linear Damping ≈ 5)`
+ `ItemPickup` bzw. `CurrencyPickup` + `PickupMagnet`

### Szene
- Ein Objekt **Systems** mit `HitStop`, `PickupSpawner`, `GameClock`
- **CameraRig** mit `CameraFollow` → Kind **Main Camera** mit `CameraShake` (+ optional *Pixel Perfect Camera*)
- **HUD-Canvas** mit `HeartDisplay`, `ResourceBar`, `WalletText`, `ClockText`, `InteractPrompt`, `DialogueBox`, `DayNightOverlay`

### Steuerung
| Aktion | Tastatur / Maus | Gamepad |
|---|---|---|
| Bewegen | WASD / Pfeiltasten | Linker Stick |
| Angriff | J / Linksklick | X / □ |
| Dash | Leertaste / K | B / ○ |
| Interagieren | E | A / ✕ |
| Werkzeug benutzen | F / Rechtsklick | Y / △ |
| Hotbar | 1–9 / Mausrad | LB / RB |

## Erweitern – Beispiele
- **Neuer Gegnerangriff** (Sprung, Flächenangriff): neue Klasse, die von `EnemyAttack` erbt und `PerformAttack` implementiert.
- **Neuer Wert** (z.B. Feuerresistenz): Eintrag in `StatType` ergänzen.
- **Neue interaktive Objekte** (Händler, Briefkasten): `IInteractable` implementieren.
- **Neue Werkzeugziele** (Angelstelle, Brunnen): `IToolTarget` implementieren.
