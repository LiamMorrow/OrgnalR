import { useState } from "react";
import { Play, Mark, ConnectedGame } from "../../Models/GameModels";
import MarkComponent from "../Mark/Mark";
import "./Game.css";

interface GameProps {
  gameId: string;
  gameState: ConnectedGame;
  attemptPlay: (play: Play) => Promise<void>;
}

function Game({ gameId, gameState, attemptPlay }: GameProps) {
  const [error, setError] = useState("");

  const attemptPlayHandler = (row: number, column: number) => () => {
    attemptPlay({
      coord: {
        column,
        row,
      },
      mark: gameState.mark,
    }).catch((e) => {
      console.error(e);
      setError("Error Making Play");
    });
  };

  const getColumn = (column: (Mark | undefined)[], rowNum: number) =>
    column.map((mark, colNum) => (
      <MarkComponent key={`${rowNum}${colNum}`} mark={mark} attemptPlay={attemptPlayHandler(rowNum, colNum)} />
    ));

  const rows = gameState.state.grid.map((row, rowNum) => getColumn(row, rowNum));

  return (
    <div className="Game">
      <div className="grid">{rows}</div>
      <span>{error}</span>
    </div>
  );
}

export default Game;
