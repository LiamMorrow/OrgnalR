using TicTacToe.Shared.Models;

namespace TicTacToe.Engine.Bot;

public interface IBot
{
    Play GetNextPlay(GameState gameState);
}
