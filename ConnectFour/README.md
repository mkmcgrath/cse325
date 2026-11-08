# U04 – Connect Four (Blazor)

A two-player Connect Four game built with Blazor (.NET 10, Interactive Server), following the
Microsoft Learn module [Build a Connect Four game with Blazor](https://learn.microsoft.com/en-us/training/modules/dotnet-connect-four/).

## Run it

```bash
cd ConnectFour
dotnet run
```

Then open the URL printed in the terminal (http://localhost:5140 by default).

## Tutorial functionality

- `Board` component (`Components/Board.razor`) with the 7×6 board, column drop buttons, and CSS drop animation (`Board.razor.css`)
- `GameState` (`GameState.cs`) registered as a singleton service in `Program.cs`. It holds the board,
  whose turn it is, all 69 winning lines, and the win/tie check
- Turn indicator, "Player X Wins!" / "It's a tie!" messages, error messages, and a reset button
- `BoardColor`, `Player1Color` and `Player2Color` component parameters (`System.Drawing.Color`) feeding CSS variables

## Additional features

1. **Scoreboard with winning streaks** (`Components/Scoreboard.razor`): total wins per player, the current
   run of consecutive wins (🔥 at 2+), each player's best streak, games played, and ties. The scores persist across
   "Reset the game" and can be cleared with **Clear scores**.
2. **Move history** (`Components/MoveHistory.razor`): every move in the current game (player, column, row),
   newest first.
3. **Pick your colors** (`Components/Pages/Home.razor`): color pickers for each player and the board, plus
   presets (Classic, Color-blind friendly, Night). The board recolors live, and the app warns you if two colors are too
   similar to tell apart.
4. **Accessibility / polish**: the four winning pieces pulse, and the column buttons are real buttons that work with
   the keyboard, have screen-reader labels, and are disabled when a column is full or the game is over. The status text is
   announced with `aria-live`, and animations respect `prefers-reduced-motion`.

## Notes on changes from the tutorial

- The tutorial's `PlayPiece` could index past the end of the board when dropping into an empty column.
  The landing loop now stops at the bottom row.
- The tutorial sets the color CSS variables in a `<HeadContent>` block. The template's `<HeadOutlet>` is rendered
  statically, so those variables never update after the page loads. They are now set as an inline style on the game container,
  which lets the color pickers work live.
