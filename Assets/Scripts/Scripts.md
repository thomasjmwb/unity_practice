# Card Game Framework Scripts

This directory contains all the scripts for your deck-based card game framework, organized into logical folders:

## Structure

- **Cards/** - Card data and effect system
- **Deck/** - Deck management and card drawing
- **Game/** - Core game logic and state management  
- **Grid/** - 3x3 grid system for card placement
- **Hand/** - Hand management and card UI
- **Modifiers/** - Global modifier system
- **Shop/** - Shopping phase and card purchasing
- **UI/** - User interface management
- **Setup/** - Helper scripts for initial setup

## Key Components

1. **Card** - ScriptableObject for card data
2. **CardEffect** - Base class for card effects
3. **GameManager** - Main game controller
4. **CardGrid** - Manages the 3x3 placement grid
5. **Deck** - Handles card drawing and shuffling
6. **ShopManager** - Manages the shopping phase

## Getting Started

1. Run the GameSetup script to create basic scene structure
2. Create Card ScriptableObjects for your game
3. Configure the UI prefabs and layouts
4. Test the game flow from drawing cards to purchasing new ones