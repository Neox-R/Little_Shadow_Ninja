═══════════════════════════════════════════════════════════════════
  COMBAT SYSTEM - Aragami 2 + Bayonetta/DMC Hybrid
═══════════════════════════════════════════════════════════════════

This combat system blends the tactical lock-on combat of Aragami 2
with the stylish, combo-heavy action of Bayonetta and Devil May Cry.

Key Features:
  • Lock-on targeting system
  • Light/Heavy attack combos (up to 5 hits)
  • Dodge with invincibility frames
  • Dodge Offset (Bayonetta-style combo extension)
  • Parry system with counter attacks
  • Perfect Dodge slow-motion
  • DMC-style ranking system (D → SSS)
  • Launcher attacks for crowd control
  • Variety bonus for mixing moves
  • Less overwhelming than Aragami 2

═══════════════════════════════════════════════════════════════════
COMPONENTS OVERVIEW
═══════════════════════════════════════════════════════════════════

PlayerCombat.cs
───────────────
  Purpose: Main combat controller for attacks
  
  Features:
    • Light attack (fast, lower damage)
    • Heavy attack (slow, high damage)
    • Attack range and cone detection
    • Automatic target facing when locked
    • Combat state detection
    • Integration with all combat systems

ComboSystem.cs
──────────────
  Purpose: Combo tracking and execution (DMC/Bayonetta-style)
  
  Features:
    • 5-hit max combo chains
    • Light (L) and Heavy (H) attack mixing
    • Combo damage multiplier (15% per hit)
    • Attack queuing (buffer next input)
    • Launcher attack at 3rd hit
    • Finisher at 5th hit (2x damage)
    • Combo timeout with extension per hit
    • Combo sequence tracking

LockOnSystem.cs
───────────────
  Purpose: Target locking (Aragami 2 style)
  
  Features:
    • Toggle lock-on with Tab
    • Auto-target closest enemy
    • Switch targets left/right
    • Lock breaks at max distance
    • Camera adjustments when locked
    • Only targets alive enemies
    • Visual indicators

DodgeSystem.cs
──────────────
  Purpose: Dodge mechanics (Bayonetta-inspired)
  
  Features:
    • Directional dodge or back dodge
    • Invincibility frames (i-frames)
    • Perfect dodge detection
    • Perfect dodge slow-motion effect
    • Dodge Offset (continue combos while dodging!)
    • Smooth arc movement
    • Cooldown between dodges

ParrySystem.cs
──────────────
  Purpose: Parry and counter mechanics
  
  Features:
    • Timed parry window (0.3s default)
    • Stuns enemy on successful parry
    • Counter attack window after parry
    • Counter deals 2.5x damage
    • Extends combo time on parry
    • Parry cooldown for balance

PlayerHealth.cs
───────────────
  Purpose: Player health management
  
  Features:
    • Health tracking
    • Damage reduction stat
    • Auto-regeneration (after delay)
    • Invincibility integration
    • Parry damage negation
    • Death/revive system
    • Event system for UI

EnemyHealth.cs
──────────────
  Purpose: Enemy health and reactions
  
  Features:
    • Health and damage tracking
    • Hitstun on attacks
    • Light vs. heavy hitstun duration
    • Knockback on hits
    • Launcher support (airborne)
    • Stun system (from parry)
    • Stun resistance stat
    • Death notifications

StyleRankSystem.cs
──────────────────
  Purpose: DMC-style ranking system
  
  Ranks: D → C → B → A → S → SS → SSS
  
  Features:
    • Style points from actions
    • Rank progression
    • Style point decay over time
    • Variety bonus (mix your moves!)
    • Combo multiplier
    • Perfect dodge/parry bonus
    • Move tracking system

CombatDebugUI.cs
────────────────
  Purpose: On-screen combat information
  
  Displays:
    • Health bar
    • Style rank (colored)
    • Style points
    • Current combo
    • Lock-on target
    • Combat state
    • Control hints

═══════════════════════════════════════════════════════════════════
SETUP INSTRUCTIONS
═══════════════════════════════════════════════════════════════════

STEP 1: Add Combat Components to Player
────────────────────────────────────────

1. Select Player GameObject
2. Add these components:
   • PlayerCombat
   • ComboSystem
   • LockOnSystem
   • DodgeSystem
   • ParrySystem
   • PlayerHealth
   • StyleRankSystem

3. All components will auto-reference each other!

STEP 2: Configure Enemy Layer
──────────────────────────────

1. Create new layer: "Enemy"
2. Assign all enemies to "Enemy" layer
3. In PlayerCombat Inspector:
   • Set "Enemy Layer" to include "Enemy"

STEP 3: Add Components to Enemies
──────────────────────────────────

1. Select Enemy GameObject
2. Add Component: EnemyHealth
3. Configure health amount
4. Optionally add Rigidbody for physics reactions

STEP 4: Setup Input Actions
────────────────────────────

Add these actions to your Input Actions asset:

  • LightAttack → Mouse Left Button
  • HeavyAttack → Mouse Right Button
  • Dodge → Space
  • Parry → Q
  • LockOn → Tab
  • SwitchTargetLeft → Mouse Scroll Up (or Q)
  • SwitchTargetRight → Mouse Scroll Down (or E)

Connect in Player Input component events:
  • LightAttack → PlayerCombat.OnLightAttack
  • HeavyAttack → PlayerCombat.OnHeavyAttack
  • Dodge → DodgeSystem.OnDodge
  • Parry → ParrySystem.OnParry
  • LockOn → LockOnSystem.OnLockOn
  • SwitchTargetLeft → LockOnSystem.OnSwitchTargetLeft
  • SwitchTargetRight → LockOnSystem.OnSwitchTargetRight

STEP 5: Add Debug UI
─────────────────────

1. Create empty GameObject: "CombatDebugUI"
2. Add Component: CombatDebugUI
3. References will auto-find Player

═══════════════════════════════════════════════════════════════════
HOW TO PLAY
═══════════════════════════════════════════════════════════════════

BASIC COMBAT
────────────
  Mouse Left: Light Attack (fast)
  Mouse Right: Heavy Attack (strong)
  
  Alternate L and H for variety!
  Example combo: L → L → H → L → H (FINISHER!)

LOCK-ON (ARAGAMI 2 STYLE)
──────────────────────────
  Tab: Toggle lock-on to nearest enemy
  Mouse Scroll / Q-E: Switch between targets
  
  While locked:
    • Auto-face target
    • Easier to land hits
    • Better for 1v1 fights

DODGE (BAYONETTA STYLE)
───────────────────────
  Space: Dodge in movement direction
  Space (no input): Back dodge
  
  Dodge just before hit = PERFECT DODGE!
    • Slow-motion effect
    • Extra style points
    • Extends combo time
  
  DODGE OFFSET:
    • Dodge during combo
    • Keeps combo alive!
    • Continue attacking after dodge
    • Like Bayonetta!

PARRY
─────
  Q: Activate parry window (0.3s)
  Time it right to parry enemy attack!
  
  On successful parry:
    • Enemy is stunned
    • Press Q again for COUNTER ATTACK!
    • Counter deals 2.5x damage
    • Free heavy hit!

STYLE RANK (DMC STYLE)
──────────────────────
  Build style points by:
    • Landing hits
    • Mixing light/heavy attacks
    • Dodging attacks
    • Perfect dodges
    • Parrying
    • Killing enemies
  
  VARIETY BONUS:
    • Use different moves
    • Don't spam same attack
    • Mix it up for bonus points!
  
  Points decay over time if you're not fighting!

═══════════════════════════════════════════════════════════════════
COMBO EXAMPLES
═══════════════════════════════════════════════════════════════════

Basic 5-Hit Combo:
  L → L → L → L → L (Finisher)
  
Heavy Finisher:
  L → L → H → H → H (Big damage finisher)
  
Launcher Combo:
  L → L → H (3rd hit = Launcher!)
  → Enemy airborne
  → Continue juggle in air
  
Dodge Offset Combo:
  L → L → [DODGE] → L → H → H
  (Combo doesn't break!)
  
Parry Counter Combo:
  [Parry enemy attack]
  → [Counter with Q]
  → L → L → H (continue combo!)

═══════════════════════════════════════════════════════════════════
ANTI-OVERWHELMING MECHANICS
═══════════════════════════════════════════════════════════════════

Unlike Aragami 2, you have tools to handle groups:

1. LAUNCHER (3rd hit)
   ─────────────────
   • Pops enemy into air
   • Removes them from fight temporarily
   • Use on one enemy while dealing with others

2. DODGE OFFSET
   ────────────
   • Dodge to safety mid-combo
   • Don't lose your damage
   • Reposition without penalty

3. LOCK-ON SWITCHING
   ──────────────────
   • Quickly switch between threats
   • Focus down weak enemies
   • Control the fight flow

4. HEAVY ATTACK KNOCKBACK
   ───────────────────────
   • Pushes enemies away
   • Creates space
   • Groups get separated

5. PARRY STUN
   ──────────
   • Stuns one enemy for 2+ seconds
   • Free to deal with others
   • Then counter for big damage

6. PERFECT DODGE
   ─────────────
   • Slow motion gives you time to think
   • Reposition during slow-mo
   • Plan next move

═══════════════════════════════════════════════════════════════════
INTEGRATION WITH STEALTH SYSTEM
═══════════════════════════════════════════════════════════════════

Combat and stealth work together:

STEALTH → COMBAT:
  • Enemies detect you
  • Combat system activates
  • Style rank starts tracking
  
COMBAT → STEALTH:
  • Defeat all enemies
  • Combat ends
  • Can return to stealth
  • Alert level may remain high

HYBRID PLAYSTYLE:
  • Stealth takedown some enemies
  • Fight the rest stylishly
  • Mix approaches as needed

═══════════════════════════════════════════════════════════════════
TUNING PARAMETERS
═══════════════════════════════════════════════════════════════════

Make Combat Easier:
  • Increase i-frame duration
  • Increase parry window
  • Decrease dodge cooldown
  • Increase player health
  • Decrease enemy health
  • Increase damage values

Make Combat Harder:
  • Decrease i-frame duration
  • Decrease parry window
  • Increase dodge cooldown
  • Decrease player health
  • Increase enemy health
  • Decrease damage values
  • Increase style decay rate

Make More Stylish (DMC-like):
  • Increase combo multipliers
  • Add more combo count
  • Increase variety bonus
  • Slower style decay

Make More Tactical (Aragami-like):
  • Lower damage values
  • Increase enemy awareness
  • Make parry timing tighter
  • Fewer i-frames

═══════════════════════════════════════════════════════════════════
FUTURE EXPANSION IDEAS
═══════════════════════════════════════════════════════════════════

Potential additions:
  ⬜ More combo moves (launchers, slams, sweeps)
  ⬜ Special abilities (Devil Trigger-style)
  ⬜ Weapon variety
  ⬜ Air combos (juggling)
  ⬜ Taunt system (DMC)
  ⬜ Finisher animations
  ⬜ Enemy variety (different behaviors)
  ⬜ Boss fights
  ⬜ Difficulty modes
  ⬜ Combat challenges/arenas
  ⬜ Unlockable moves
  ⬜ Skill tree

═══════════════════════════════════════════════════════════════════
TROUBLESHOOTING
═══════════════════════════════════════════════════════════════════

❌ Attacks not working:
   → Check Input Actions are connected
   → Verify enemy layer is set correctly
   → Ensure enemies have colliders

❌ Lock-on not finding targets:
   → Enemies must have EnemyHealth component
   → Enemies must be on "Enemy" layer
   → Check lock-on range

❌ Dodge not giving i-frames:
   → Check iFramesDuration value
   → Ensure DodgeSystem is attached

❌ Combo not chaining:
   → Press next attack during combo window
   → Check combo timeout values
   → Don't spam too fast

❌ Style rank not increasing:
   → Land hits on enemies
   → Mix up your moves
   • Check StyleRankSystem is attached

═══════════════════════════════════════════════════════════════════

Enjoy your stylish ninja combat! 🥷⚔️✨

Aragami stealth + Bayonetta combos = The best of both worlds!
