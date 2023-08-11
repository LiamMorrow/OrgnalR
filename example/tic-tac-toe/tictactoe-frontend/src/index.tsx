import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import App, { AppProps } from "./App";
import reportWebVitals from "./reportWebVitals";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { Play, Mark, ConnectedGame } from "./Models/GameModels";
import { config } from "./config";

const gameHubConnection = new HubConnectionBuilder()
  .withAutomaticReconnect()
  .withUrl(config.endpoints[Math.floor(Math.random() * config.endpoints.length)])
  .build();

let notifyOfNewGameState: undefined | ((gameId: string) => void);

gameHubConnection.start().then(() =>
  gameHubConnection.on("NewGameStateAvailable", async (gameId: string) => {
    notifyOfNewGameState && notifyOfNewGameState(gameId);
  }),
);

const joinGame = (gameId: string) =>
  gameHubConnection.invoke<Mark>("JoinGame", {
    gameId,
  });

const getGameState = (gameId: string) => gameHubConnection.invoke<ConnectedGame>("GetCurrentGameState", { gameId });

const attemptPlay = (gameId: string, play: Play) =>
  gameHubConnection.invoke<void>("AttemptPlay", {
    gameId,
    play,
  });
const addBot = (gameId: string) =>
  gameHubConnection.invoke<void>("AddBot", {
    gameId,
  });

const appProps: AppProps = {
  joinGame,
  getGameState,
  attemptPlay,
  addBot,
  setNotifyOfNewGameStateCb: (cb) => (notifyOfNewGameState = cb),
};

const root = ReactDOM.createRoot(document.getElementById("root") as HTMLElement);
root.render(
  <React.StrictMode>
    <App {...appProps} />
  </React.StrictMode>,
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
