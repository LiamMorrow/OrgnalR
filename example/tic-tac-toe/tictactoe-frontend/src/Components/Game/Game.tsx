import { useState } from "react";
import { Play, Mark, ConnectedGame } from "../../Models/GameModels";
import MarkComponent from "../Mark/Mark";
import "./Game.css";

interface GameProps {
  connectedGame: ConnectedGame;
  attemptPlay: (play: Play) => Promise<void>;
  addBot: () => Promise<void>;
}

function Game({ addBot, connectedGame, attemptPlay }: GameProps) {
  const [error, setError] = useState("");

  const attemptPlayHandler = (row: number, column: number) => () => {
    attemptPlay({
      coord: {
        column,
        row,
      },
      mark: connectedGame.mark,
    }).catch((e) => {
      console.error(e);
      setError("Error Making Play");
    });
  };

  const getColumn = (column: (Mark | undefined)[], rowNum: number) =>
    column.map((mark, colNum) => (
      <MarkComponent key={`${rowNum}${colNum}`} mark={mark} attemptPlay={attemptPlayHandler(rowNum, colNum)} />
    ));

  const rows = connectedGame.state.grid.map((row, rowNum) => getColumn(row, rowNum));

  return (
    <div className="Game">
      <div className="grid">{rows}</div>
      <span>{error}</span>
      {!connectedGame.opponent && <button onClick={addBot}>Add Opponent Bot</button>}
    </div>
  );
}

export default Game;
