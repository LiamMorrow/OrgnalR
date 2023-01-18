import React, { useEffect, useState } from "react";
import "./App.css";
import { Play, Mark, ConnectedGame } from "./Models/GameModels";
import Game from "./Components/Game/Game";
import JoinGame from "./Components/JoinGame/JoinGame";

interface RunningGameState {
  gameId: string;
  gameState: ConnectedGame;
}

export interface AppProps {
  setNotifyOfNewGameStateCb: (cb: (gameId: string) => void) => void;
  joinGame: (gameId: string) => Promise<Mark>;
  attemptPlay: (gameId: string, play: Play) => Promise<void>;
  getGameState: (gameId: string) => Promise<ConnectedGame>;
  addBot: (gameId: string) => Promise<void>;
}

function App({ setNotifyOfNewGameStateCb, joinGame, attemptPlay, addBot, getGameState }: AppProps) {
  const [runningGameState, setRunningGameState] = useState<RunningGameState | undefined>(undefined);

  useEffect(() => {
    setNotifyOfNewGameStateCb(async (gameId) => {
      if (runningGameState?.gameId === gameId) {
        setRunningGameState({
          ...runningGameState,
          gameId,
          gameState: await getGameState(gameId),
        });
      }
    });
  }, [runningGameState, setNotifyOfNewGameStateCb, getGameState]);

  const joinGameHandler = (gameId: string) =>
    joinGame(gameId).then(async (mark) =>
      setRunningGameState({
        gameId,
        gameState: await getGameState(gameId),
      }),
    );

  const attemptPlayHandler = async (play: Play) => {
    if (!runningGameState) {
      return;
    }
    await attemptPlay(runningGameState.gameId, play);
  };
  const addBotHandler = async () => {
    if (!runningGameState) {
      return;
    }
    await addBot(runningGameState.gameId);
  };

  return (
    <div className="App">
      <header className="App-header">Tic Tac Toe</header>
      <main>
        {runningGameState ? (
          <Game connectedGame={runningGameState.gameState} attemptPlay={attemptPlayHandler} addBot={addBotHandler} />
        ) : (
          <JoinGame joinGame={joinGameHandler} />
        )}
      </main>
    </div>
  );
}

export default App;
