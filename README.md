# Mirror's Edge LRT - AutoSplitter and Load Remover

### Features:

  - Things [Fatalis' timer](https://github.com/fatalis/LiveSplit.MirrorsEdge) did:

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
    - Save Icons
    - White Screens
    - 5B infinite timer pause is fixed
    - more accurately times elevators (pauses only when streaming blocks an elevator from moving/does not pause for the fixed delays between loading & unloading in some elevators)
    - the timer will pause if the player tabs/opens the escape menu during level streaming and it will stay paused until the player exits the menu

### Features to be added

- Splitting on bag collection in 100%/disabling chapter splits in 100%
- Custom starting time trial for 69 Stars

### Known Issues

- Timer can appear laggy at times, which seems to be a LiveSplit+Medge issue and not directly related to the component (this seems like it can be temporarily fixed by restarting the game)

- Potential Issue: It is not yet clear if multiple different versions of the binkw32.dll exist in different game installations, Steam and DVD are confirmed to have the same version

### Installation

Put the LiveSplit.MirrorsEdgeLRT.dll from the latest [release](https://github.com/Ph03n1xDE/LiveSplit.MirrorsEdgeLRT/releases/latest) in your LiveSplit/Components folder. Then edit your layout and add the component from the control section.

The currently supported versions are: Steam, Reloaded and Origin. If your version is not supported, please ask in the Mirror's Edge Speedrunning [Discord](https://discord.gg/3tbaHJg).

