═══════════════════════════════════════════════════════════════════
  DIAPER SYSTEM - IMPLEMENTATION GUIDE
═══════════════════════════════════════════════════════════════════

This folder contains the ABDL mechanics for Little Shadow Ninja.
These systems integrate with the existing stealth mechanics to create
resource management and additional stealth challenges.

═══════════════════════════════════════════════════════════════════
COMPONENTS OVERVIEW
═══════════════════════════════════════════════════════════════════

DiaperState.cs
──────────────
  Purpose: Tracks diaper wetness and mess levels
  
  Key Features:
    • Separate wetness and mess capacity
    • Leak detection when thresholds reached
    • Movement speed penalty calculation
    • Smell intensity calculation
    • Condition states (Clean → Soaked/Messy → Full)
  
  Events:
    • OnWetnessChanged(float)
    • OnMessChanged(float)
    • OnConditionChanged(DiaperCondition)
    • OnStartedLeaking()

BladderBowelSystem.cs
─────────────────────
  Purpose: Manages bladder and bowel filling over time
  
  Key Features:
    • Automatic filling based on time
    • Manual release via input (P key)
    • Automatic ACCIDENT when full (after 2s delay)
    • Player freezes/stops during accident (3 seconds)
    • Accident gradually releases over duration
    • Cannot use diaper voluntarily during accident
    • Accepts consumable items to fill levels
  
  Usage:
    • Press P to voluntarily use diaper
    • If bladder/bowel full → 2s warning → ACCIDENT (player stops for 3s)

ConsumableItem.cs
─────────────────
  Purpose: Collectible food/drink items
  
  Item Types:
    • Drink → Fills bladder
    • Food → Fills bowel
    • Both → Fills both
  
  Setup:
    • Attach to GameObject with trigger collider
    • Set bladderFillAmount and/or bowelFillAmount
    • Player walks over to collect

DiaperSmellDetection.cs
───────────────────────
  Purpose: Enemy AI extension for smell-based detection
  
  Key Features:
    • Normal smell range (default 5m)
    • Through-walls detection range (default 3m)
    • Smell intensity based on diaper state
    • Automatic awareness increase
  
  How It Works:
    • Messy diaper = high smell
    • Leaking diaper = 2x smell multiplier
    • Clean/slightly wet = no smell
    • Enemies detect through walls at close range

PlayerDiaperIntegration.cs
──────────────────────────
  Purpose: Integrates diaper system with existing player systems
  
  Features:
    • Movement speed penalties
    • Visibility modifiers (smell-based)
    • Event handling and feedback
  
  Integration:
    • Automatically adjusts PlayerController speeds
    • Affects PlayerVisibility calculations

DiaperDebugUI.cs
────────────────
  Purpose: On-screen debug display
  
  Shows:
    • Bladder and bowel levels
    • Diaper wetness and mess
    • Current condition
    • Smell intensity
    • Speed penalty
    • Controls reminder

═══════════════════════════════════════════════════════════════════
SETUP INSTRUCTIONS
═══════════════════════════════════════════════════════════════════

STEP 1: Add Components to Player
─────────────────────────────────

1. Select your Player GameObject
2. Add these components in order:
   • DiaperState
   • BladderBowelSystem
   • PlayerDiaperIntegration

3. In BladderBowelSystem Inspector:
   • Assign DiaperState reference (drag from components)

4. In PlayerDiaperIntegration Inspector:
   • Assign PlayerController reference
   • Assign PlayerVisibility reference
   • Assign DiaperState reference
   • Assign BladderBowelSystem reference

STEP 2: Configure Input
────────────────────────

1. Open your Input Actions asset (or create one)
2. Add new action: "Potty"
   • Binding: P key
   • Action Type: Button
3. In Player GameObject:
   • Find PlayerInput component
   • Add event: Potty → BladderBowelSystem.OnPottyInput

STEP 3: Add to AlertManager (or Scene Manager)
───────────────────────────────────────────────

1. Select AlertManager GameObject
2. Add Component: DiaperDebugUI
3. References auto-find Player, but you can assign manually

STEP 4: Add to Enemies (Optional but Recommended)
──────────────────────────────────────────────────

1. Select each Enemy GameObject
2. Add Component: DiaperSmellDetection
3. In Inspector:
   • Awareness reference should auto-assign
   • Set Wall Layers to include "Cover" layer
   • Adjust smell ranges as desired

STEP 5: Create Consumable Items
────────────────────────────────

DRINK ITEM:
  1. GameObject > 3D Object > Sphere
  2. Name: "Drink_WaterBottle"
  3. Scale: (0.3, 0.3, 0.3)
  4. Add Component > Sphere Collider (is Trigger = true)
  5. Add Component > Consumable Item
  6. Set:
     • Type: Drink
     • Bladder Fill Amount: 20
     • Bowel Fill Amount: 0
     • Item Color: Blue

FOOD ITEM:
  1. GameObject > 3D Object > Cube
  2. Name: "Food_Snack"
  3. Scale: (0.4, 0.2, 0.3)
  4. Add Component > Box Collider (is Trigger = true)
  5. Add Component > Consumable Item
  6. Set:
     • Type: Food
     • Bladder Fill Amount: 0
     • Bowel Fill Amount: 25
     • Item Color: Brown/Orange

═══════════════════════════════════════════════════════════════════
GAMEPLAY MECHANICS
═══════════════════════════════════════════════════════════════════

BLADDER & BOWEL FILLING
────────────────────────
  • Both fill slowly over time (passive)
  • Collecting drinks/food speeds up filling
  • When full → 2 second warning, then ACCIDENT happens
  • During accident → Player STOPS and cannot move for 3 seconds
  • Strategy: Manage consumption vs. current diaper state, use diaper before it's too late!

DIAPER CAPACITY
────────────────
  • Wetness Max: 100 units
  • Mess Max: 100 units
  • Each can fill independently
  • Leak threshold: 90% (default)
  • Full = 100% (can't hold more)

MOVEMENT PENALTIES
──────────────────
  • Clean: 100% speed (no penalty)
  • Only Wet (>50%): 50% speed
  • Only Messy (>50%): 50% speed
  • Completely Full: 30% speed
  • Gradual scaling between thresholds

STEALTH IMPLICATIONS
─────────────────────
  • Clean/Slightly Used: No smell, normal stealth
  • Messy: High smell, enemies can detect
  • Leaking: 2x smell multiplier, very dangerous!
  • Very Full: Slow movement makes escaping hard
  
RISK VS. REWARD
────────────────
  • Consumables may be needed (healing, buffs, etc.)
  • But consuming = filling bladder/bowel
  • Must balance: "Do I take this drink now or later?"
  • Full diaper = slow + smelly = high risk

═══════════════════════════════════════════════════════════════════
TUNING PARAMETERS
═══════════════════════════════════════════════════════════════════

Make Harder (More Challenge):
  • Increase bladder/bowel fill rates
  • Decrease diaper capacities
  • Increase smell detection ranges
  • Make consumables fill more

Make Easier (More Forgiving):
  • Decrease fill rates
  • Increase diaper capacities
  • Decrease smell ranges
  • Reduce movement penalties

Balanced Default Values:
  • Bladder fills in ~100 seconds
  • Bowel fills in ~200 seconds
  • Diaper holds ~3-4 full releases
  • Smell range: 5m normal, 3m through walls

═══════════════════════════════════════════════════════════════════
INTEGRATION WITH EXISTING SYSTEMS
═══════════════════════════════════════════════════════════════════

PlayerController
────────────────
  • Movement speeds affected by diaper state
  • No code changes needed (modular!)

PlayerVisibility
────────────────
  • Smell adds to visibility when detected
  • Integrates through PlayerDiaperIntegration

EnemyAwareness
──────────────
  • Receives awareness from smell detection
  • Same state machine (Idle → Suspicious → Alert)

EnemyVision
───────────
  • Works alongside smell detection
  • Can be detected by vision OR smell

AlertManager
────────────
  • Smell-based alerts propagate same way
  • No changes needed

═══════════════════════════════════════════════════════════════════
FUTURE EXTENSIONS
═══════════════════════════════════════════════════════════════════

Possible Additions:
  ⬜ Diaper changing stations in levels
  ⬜ Different diaper types (capacity, leak threshold)
  ⬜ Clothing over diaper (visibility reduction)
  ⬜ Comfort/discomfort system affecting stats
  ⬜ NPCs commenting on player's state
  ⬜ Achievements/stats tracking
  ⬜ Progressive difficulty (fill rates increase)
  ⬜ Safe zones for changing
  ⬜ Risk of being discovered while changing

═══════════════════════════════════════════════════════════════════
TROUBLESHOOTING
═══════════════════════════════════════════════════════════════════

❌ Input not working:
   → Check Input Actions asset has "Potty" action
   → Verify PlayerInput component has event connected
   → Try pressing P key while watching Console

❌ No automatic release when full:
   → Check bladder/bowel levels in debug UI
   → Wait 2 seconds after reaching 100%
   → Verify DiaperState reference is assigned

❌ Enemies not detecting smell:
   → Add DiaperSmellDetection to enemy
   → Check diaper is messy/leaking (debug UI)
   → Verify Wall Layers mask is set correctly

❌ No movement penalty:
   → Check "Apply Movement Penalty" in PlayerDiaperIntegration
   → Verify diaper state is actually full (debug UI)
   → Check PlayerController reference is assigned

❌ Debug UI not showing:
   → Toggle "Show Debug UI" checkbox
   → Verify DiaperDebugUI is on a GameObject in scene
   → Check references are assigned

═══════════════════════════════════════════════════════════════════

Privacy Note:
This folder name is intentionally obvious as a "do not open" indicator.
If working with others, you can:
  • Tell them it's personal/private test code
  • Say it's placeholder mechanics being replaced
  • Keep the scripts disabled until needed
  • Create a toggle to enable/disable entire system

The system is fully modular - just don't add the components to Player
and none of this will run!

═══════════════════════════════════════════════════════════════════
