# Mirror's Edge LRT - Load Remover and Auto Splitter

### Features:

  - Things Fatalis' timer did:

    - Between-level cutscenes.
    - Animated loading screens.
    - Elevators, i.e. hidden loading screens
    - Loading caused by deaths
    - Other special cases
    - Stormdrains beeg doors
    - Stormdrains exit button
    - Chapter 4 skip
    - Chapter 6 skip
    - Chapter 9F elevator lobby

  - Things Fatalis' timer did not do:

    - Block While Loading
    - Save Icons (see known issues)
    - White Screens
    - 5B infinite timer pause is fixed
    - More accurately times elevators (pauses only when streaming blocks an elevator from moving/does not pause for the fixed delays between loading & unloading in some elevators)
    - The timer will pause if the player tabs/opens the escape menu during level streaming and it will stay paused until the player exits the menu
    - Support for splitting in 69 Stars
    - Custom starting time trial for 69 Stars
    - Splitting on every bag in 100% (with option to disable chapter splits)
    
### Features to be added:

- Splitting on checkpoints (unlikely for now)


### Known Issues:

- Timer can appear laggy at times, which seems to be a LiveSplit+Medge issue and not directly related to this component (can usually be temporarily fixed by restarting the game)

### Installation:

Put the LiveSplit.MirrorsEdgeLRT.dll in your LiveSplit/Components folder. Then edit your layout and add the component from the control section.
