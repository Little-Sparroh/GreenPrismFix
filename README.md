# GreenPrismFix

A BepInEx mod for MycoPunk that fixes the MiniCannon (Green Prism) prism upgrade application bugs.

## Description

This client-side mod addresses a critical issue with MiniCannon prism upgrades that was causing stat bonuses to not apply correctly, or apply partially. The Green Prism weapon uses a complex prism connectivity system where multiple prism upgrades create synergy bonuses, but the original implementation had bugs that prevented proper stat scaling.

The mod patches the `UpgradeProperty_MiniCannon_Prism.Apply` method to ensure prism upgrades are applied with the correct multiplicative scaling based on both upgrade rarity and the number of connected prisms. Different rarity levels affect different weapon stats: Standard prisms increase range, Rare affects spread, Epic improves fire rate, and Exotic boosts damage.

## Getting Started

### Dependencies

* MycoPunk (base game)
* [BepInEx](https://github.com/BepInEx/BepInEx) - Version 5.4.2403 or compatible
* .NET Framework 4.8

### Building/Compiling

1. Clone this repository
2. Open the solution file in Visual Studio, Rider, or your preferred C# IDE
3. Build the project in Release mode

Alternatively, use dotnet CLI:
```bash
dotnet build --configuration Release
```

### Installing

**Option 1: Via Thunderstore (Recommended)**
1. Download and install using the Thunderstore Mod Manager
2. Search for "GreenPrismFix" under MycoPunk community
3. Install and enable the mod (Note: This mod is marked as deprecated)

**Option 2: Manual Installation**
1. Ensure BepInEx is installed for MycoPunk
2. Copy `GreenPrismFix.dll` from the build folder
3. Place it in `<MycoPunk Game Directory>/BepInEx/plugins/`
4. Launch the game

### Executing program

Once installed, prism upgrades on MiniCannons will work correctly:

**Prism Upgrade Effects:**
- **Standard Prisms:** Increase maximum damage range
- **Rare Prisms:** Reduce bullet spread
- **Epic Prisms:** Decrease fire interval (faster shooting)
- **Exotic Prisms:** Multiply base damage

**Connectivity Bonus:**
- Each additional connected prism increases the effect multiplicatively
- The more prisms you connect, the stronger the overall bonuses
- Connectivity is determined by upgrade placement logic

## Help

* **Prism effects still not working?** This mod only affects prism upgrade application - check upgrade rarity and placement
* **No connectivity bonuses?** Ensure prisms are properly positioned to create connections
* **Conflicts with other mods?** Other mods modifying MiniCannon stats may interfere
* **Performance impact?** Minimal - only patches upgrade application logic
* **Affects other weapons?** Only modifies MiniCannon/Green Prism prism upgrades

## Authors

* Sparroh
* funlennysub (original mod template)
* [@DomPizzie](https://twitter.com/dompizzie) (README template)

## License

* This project is licensed under the MIT License - see the LICENSE.md file for details
