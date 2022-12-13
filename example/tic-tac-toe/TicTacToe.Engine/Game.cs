using TicTacToe.Shared.Models;
using System.Linq;

namespace TicTacToe.Engine;

public class Game
{
    const int GRID_SIZE = 3;
    public GameState CurrentState { get; private set; }

    public Game(GameState state)
    {
        this.CurrentState = state;
    }

    public Game()
        : this(
            new GameState(
                Turn: Symbol.X,
                Grid: Enumerable
                    .Range(0, GRID_SIZE)
                    .Select((_) => new Symbol?[GRID_SIZE])
                    .ToArray(),
                Winner: null,
                Draw: false
            )
        ) { }

    public void AttemptPlay(Play play)
    {
        var state = CurrentState;
        if (state.Winner != null || state.Draw)
        {
            throw new InvalidOperationException("Game is over");
        }
        if (state.Turn != play.Symbol)
        {
            throw new InvalidOperationException($"It is currently {state.Turn}'s turn");
        }
        if (play.Coord is { Row: < 0 or > 2, Column: < 0 or > 2 })
        {
            throw new InvalidOperationException(
                "Out of bounds. Row and Column must be between 0 and 2"
            );
        }

        if (state.Grid[play.Coord.Row][play.Coord.Column] != null)
        {
            throw new InvalidOperationException("Already a symbol in that tile");
        }

        var newGrid = new Symbol?[GRID_SIZE][];
        for (var row = 0; row < GRID_SIZE; row++)
        {
            newGrid[row] = new Symbol?[GRID_SIZE];
            for (var col = 0; col < GRID_SIZE; col++)
            {
                newGrid[row][col] = state.Grid[row][col];
                if (row == play.Coord.Row && col == play.Coord.Column)
                {
                    newGrid[row][col] = play.Symbol;
                }
            }
        }
        var (WinningPlay, Draw) = IsGameOver(newGrid, play);
        CurrentState = state with
        {
            Grid = newGrid,
            Turn = state.Turn == Symbol.O ? Symbol.X : Symbol.O,
            Winner = WinningPlay ? play.Symbol : null,
            Draw = Draw
        };
    }

    // Adapted from https://stackoverflow.com/questions/1056316/algorithm-for-determining-tic-tac-toe-game-over
    private static (bool WinningPlay, bool Draw) IsGameOver(Symbol?[][] grid, Play play)
    {
        var (symbol, (row, col)) = play;
        //check row
        for (int i = 0; i < GRID_SIZE; i++)
        {
            if (grid[row][i] != symbol)
                break;
            if (i == GRID_SIZE - 1)
            {
                return (true, false);
            }
        }

        //check col
        for (int i = 0; i < GRID_SIZE; i++)
        {
            if (grid[i][col] != symbol)
                break;
            if (i == GRID_SIZE - 1)
            {
                return (true, false);
            }
        }

        //check diag
        if (row == col)
        {
            //we're on a diagonal
            for (int i = 0; i < GRID_SIZE; i++)
            {
                if (grid[row][col] != symbol)
                    break;
                if (i == GRID_SIZE - 1)
                {
                    return (true, false);
                }
            }
        }

        //check anti diag (thanks rampion)
        if (row + col == GRID_SIZE - 1)
        {
            for (int i = 0; i < GRID_SIZE; i++)
            {
                if (grid[i][(GRID_SIZE - 1) - i] != symbol)
                    break;
                if (i == GRID_SIZE - 1)
                {
                    return (true, false);
                }
            }
        }

        var draw = true;
        foreach (var setSymbol in grid)
        {
            draw = draw && setSymbol != null;
        }

        return (false, draw);
    }
}
