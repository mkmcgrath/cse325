namespace ConnectFour;

/// <summary>
/// One piece that was dropped during the current game.
/// </summary>
/// <param name="Number">1-based move number</param>
/// <param name="Player">Player who made the move (1 or 2)</param>
/// <param name="Column">0-indexed column the piece was dropped into</param>
/// <param name="Row">1-indexed row the piece landed on (1 = top, 6 = bottom)</param>
/// <param name="BoardIndex">Index into <see cref="GameState.TheBoard"/> (0-41)</param>
public record Move(int Number, int Player, byte Column, byte Row, int BoardIndex);

public class GameState
{
    static GameState()
    {
        CalculateWinningPlaces();
    }

    /// <summary>
    /// Indicate whether a player has won, the game is a tie, or game in ongoing
    /// </summary>
    public enum WinState
    {
        No_Winner = 0,
        Player1_Wins = 1,
        Player2_Wins = 2,
        Tie = 3
    }

    /// <summary>
    /// The player whose turn it is.  By default, player 1 starts first
    /// </summary>
    public int PlayerTurn => TheBoard.Count(x => x != 0) % 2 + 1;

    /// <summary>
    /// Number of turns completed and pieces played so far in the game
    /// </summary>
    public int CurrentTurn { get { return TheBoard.Count(x => x != 0); } }

    public static readonly List<int[]> WinningPlaces = new();

    public static void CalculateWinningPlaces()
    {
        // Horizontal rows
        for (byte row = 0; row < 6; row++)
        {
            byte rowCol1 = (byte)(row * 7);
            byte rowColEnd = (byte)((row + 1) * 7 - 1);
            byte checkCol = rowCol1;
            while (checkCol <= rowColEnd - 3)
            {
                WinningPlaces.Add(new int[] {
                    checkCol,
                    (byte)(checkCol + 1),
                    (byte)(checkCol + 2),
                    (byte)(checkCol + 3)
                });
                checkCol++;
            }
        }

        // Vertical Columns
        for (byte col = 0; col < 7; col++)
        {
            byte checkRow = col;
            while (checkRow <= 14 + col)
            {
                WinningPlaces.Add(new int[] {
                    checkRow,
                    (byte)(checkRow + 7),
                    (byte)(checkRow + 14),
                    (byte)(checkRow + 21)
                });
                checkRow += 7;
            }
        }

        // forward slash diagonal "/"
        for (byte col = 0; col < 4; col++)
        {
            // starting column must be 0-3
            byte colRow1 = (byte)(21 + col);
            byte colRowEnd = (byte)(35 + col);
            byte checkPos = colRow1;
            while (checkPos <= colRowEnd)
            {
                WinningPlaces.Add(new int[] {
                    checkPos,
                    (byte)(checkPos - 6),
                    (byte)(checkPos - 12),
                    (byte)(checkPos - 18)
                });
                checkPos += 7;
            }
        }

        // back slash diagonal "\"
        for (byte col = 0; col < 4; col++)
        {
            // starting column must be 0-3
            byte colRow1 = (byte)(0 + col);
            byte colRowEnd = (byte)(14 + col);
            byte checkPos = colRow1;
            while (checkPos <= colRowEnd)
            {
                WinningPlaces.Add(new int[] {
                    checkPos,
                    (byte)(checkPos + 8),
                    (byte)(checkPos + 16),
                    (byte)(checkPos + 24)
                });
                checkPos += 7;
            }
        }
    }

    /// <summary>
    /// Check the state of the board for a winning scenario
    /// </summary>
    /// <returns>0 - no winner, 1 - player 1 wins, 2 - player 2 wins, 3 - draw</returns>
    public WinState CheckForWin()
    {
        // Exit immediately if less than 7 pieces are played
        if (TheBoard.Count(x => x != 0) < 7) return WinState.No_Winner;

        foreach (var scenario in WinningPlaces)
        {
            if (TheBoard[scenario[0]] == 0) continue;

            if (TheBoard[scenario[0]] == TheBoard[scenario[1]] &&
                TheBoard[scenario[1]] == TheBoard[scenario[2]] &&
                TheBoard[scenario[2]] == TheBoard[scenario[3]]) return (WinState)TheBoard[scenario[0]];
        }

        if (TheBoard.Count(x => x != 0) == 42) return WinState.Tie;

        return WinState.No_Winner;
    }

    /// <summary>
    /// The four board positions that make up the winning line, or an empty
    /// array when nobody has won yet.
    /// </summary>
    public int[] WinningPositions
    {
        get
        {
            foreach (var scenario in WinningPlaces)
            {
                var first = TheBoard[scenario[0]];
                if (first != 0 && scenario.All(pos => TheBoard[pos] == first)) return scenario;
            }
            return Array.Empty<int>();
        }
    }

    /// <summary>
    /// True when the top slot of the column already has a piece in it
    /// </summary>
    public bool IsColumnFull(byte column) => TheBoard[column] != 0;

    /// <summary>
    /// Takes the current turn and places a piece in the 0-indexed column requested
    /// </summary>
    /// <param name="column">0-indexed column to place the piece into</param>
    /// <returns>The final row (1 = top, 6 = bottom) where the piece resides</returns>
    public byte PlayPiece(byte column)
    {
        // Check for a current win
        if (CheckForWin() != 0) throw new ArgumentException("Game is over");

        // Check the column
        if (TheBoard[column] != 0) throw new ArgumentException("Column is full");

        // Drop the piece in: slide down while the slot below is on the board and empty
        var landingSpot = column;
        while (landingSpot + 7 < 42 && TheBoard[landingSpot + 7] == 0)
        {
            landingSpot = (byte)(landingSpot + 7);
        }

        var player = PlayerTurn;
        TheBoard[landingSpot] = player;

        var row = ConvertLandingSpotToRow(landingSpot);
        Moves.Add(new Move(Moves.Count + 1, player, column, row, landingSpot));

        // Update the scoreboard as soon as the game ends
        var result = CheckForWin();
        if (result != WinState.No_Winner) RecordResult(result);

        return row;
    }

    public List<int> TheBoard { get; private set; } = new List<int>(new int[42]);

    public void ResetBoard()
    {
        TheBoard = new List<int>(new int[42]);
        Moves = new List<Move>();
    }

    private byte ConvertLandingSpotToRow(int landingSpot)
    {
        return (byte)(Math.Floor(landingSpot / (decimal)7) + 1);
    }

    // ------------------------------------------------------------------
    // Additional features: move history and scoreboard / win streaks
    // ------------------------------------------------------------------

    /// <summary>
    /// Every move made in the current game, in order
    /// </summary>
    public List<Move> Moves { get; private set; } = new();

    /// <summary>
    /// Total wins per player. Index 0 is unused so Wins[1] / Wins[2] read naturally.
    /// </summary>
    public int[] Wins { get; } = new int[3];

    /// <summary>
    /// Current run of consecutive wins per player (resets on a loss or tie)
    /// </summary>
    public int[] CurrentStreak { get; } = new int[3];

    /// <summary>
    /// Longest run of consecutive wins per player this session
    /// </summary>
    public int[] BestStreak { get; } = new int[3];

    public int Ties { get; private set; }

    public int GamesPlayed => Wins[1] + Wins[2] + Ties;

    private void RecordResult(WinState result)
    {
        if (result == WinState.Tie)
        {
            Ties++;
            CurrentStreak[1] = 0;
            CurrentStreak[2] = 0;
            return;
        }

        var winner = (int)result;
        var loser = winner == 1 ? 2 : 1;

        Wins[winner]++;
        CurrentStreak[winner]++;
        CurrentStreak[loser] = 0;
        BestStreak[winner] = Math.Max(BestStreak[winner], CurrentStreak[winner]);
    }

    /// <summary>
    /// Clear all wins, ties and streaks
    /// </summary>
    public void ResetScores()
    {
        Array.Clear(Wins);
        Array.Clear(CurrentStreak);
        Array.Clear(BestStreak);
        Ties = 0;
    }
}
