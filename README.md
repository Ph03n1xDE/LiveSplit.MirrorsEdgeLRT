Features:

- Covers (at least we hope/have seen in testing):

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
    - more accurately times elevators (pauses only when streaming blocks an elevator from moving/does not pause for the fixed delays between loading & unloading in some elevators)
    - the timer will pause if the player tabs/opens the escape menu during level streaming and it will stay paused until the player exits the menu

Features to be added:

- Splitting on bag collection in 100%/disabling chapter splits in 100%
- Custom starting time trial for 69 Stars

Known Issues:

- Timer can appear laggy at times, which seems to be a LiveSplit+Medge issue and not directly related to our component (this seems like it can be temporarily fixed by restarting the game)

- Potential Issue: It is not yet clear if multiple different versions of the binkw32.dll exist in different game installations, Steam and DVD are confirmed to have the same version

Installation:

Put the LiveSplit.MirrorsEdgeLRT.dll in your LiveSplit/Components folder. Then edit your layout and add the component from the control section.
