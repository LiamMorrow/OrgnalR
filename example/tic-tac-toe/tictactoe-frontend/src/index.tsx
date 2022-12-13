import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import App, { AppProps } from "./App";
import reportWebVitals from "./reportWebVitals";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { GameState, Play, Symbl } from "./Models/GameModels";
import { v4 as uuidv4 } from "uuid";

const gameHubConnection = new HubConnectionBuilder()
  .withAutomaticReconnect()
  .withUrl("http://localhost:5120/game")
  .build();

let notifyOfNewGameState: undefined | ((gameId: string) => void);

gameHubConnection.start().then(() =>
  gameHubConnection.on("NewGameStateAvailable", async (gameId) => {
    notifyOfNewGameState && notifyOfNewGameState(gameId);
  })
);

const userId = localStorage.getItem("USER_ID") ?? uuidv4();
localStorage.setItem("USER_ID", userId);

const joinGame = (gameId: string) =>
  gameHubConnection.invoke<Symbl>("JoinGame", {
    gameId,
    userId,
  });

const getGameState = (gameId: string) => gameHubConnection.invoke<GameState>("GetCurrentGameState", gameId);

const attemptPlay = (gameId: string, play: Play) =>
  gameHubConnection.invoke<void>("AttemptPlay", {
    gameId,
    userId,
    play,
  });

const appProps: AppProps = {
  joinGame,
  getGameState,
  attemptPlay,
  notifyOfNewGameStateCb: (cb) => (notifyOfNewGameState = cb),
};

const root = ReactDOM.createRoot(document.getElementById("root") as HTMLElement);
root.render(
  <React.StrictMode>
    <App {...appProps} />
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
