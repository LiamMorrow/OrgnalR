import React, { useEffect, useState } from "react";
import "./App.css";
import { GameState, Play, Symbl } from "./Models/GameModels";
import Game from "./Components/Game/Game";
import JoinGame from "./Components/JoinGame/JoinGame";

interface RunningGameState {
  gameId: string;
  gameState: GameState;
  symbl: Symbl;
}

export interface AppProps {
  notifyOfNewGameStateCb: (cb: (gameId: string) => void) => void;
  joinGame: (gameId: string) => Promise<Symbl>;
  attemptPlay: (gameId: string, play: Play) => Promise<void>;
  getGameState: (gameId: string) => Promise<GameState>;
}

function App({ notifyOfNewGameStateCb, joinGame, attemptPlay, getGameState }: AppProps) {
  const [runningGameState, setRunningGameState] = useState<RunningGameState | undefined>(undefined);

  useEffect(() => {
    notifyOfNewGameStateCb(async (gameId) => {
      if (runningGameState?.gameId === gameId) {
        setRunningGameState({
          ...runningGameState,
          gameId,
          gameState: await getGameState(gameId),
        });
      }
    });
  }, [runningGameState, notifyOfNewGameStateCb, getGameState]);

  const joinGameHandler = (gameId: string) =>
    joinGame(gameId).then(async (symbl) =>
      setRunningGameState({
        gameId,
        symbl,
        gameState: await getGameState(gameId),
      })
    );

  const attemptPlayHandler = async (play: Play) => {
    if (!runningGameState) {
      return;
    }
    await attemptPlay(runningGameState.gameId, play);
  };

  return (
    <div className="App">
      <header className="App-header">Tic Tac Toe</header>
      <main>
        {runningGameState ? (
          <Game
            gameId={runningGameState.gameId}
            gameState={runningGameState.gameState}
            attemptPlay={attemptPlayHandler}
            symbol={runningGameState.symbl}
          />
        ) : (
          <JoinGame joinGame={joinGameHandler} />
        )}
      </main>
    </div>
  );
}

export default App;
