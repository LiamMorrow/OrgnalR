import { useState } from "react";
import { GameState, Play, Mark } from "../../Models/GameModels";
import MarkComponent from "../Mark/Mark";
import "./Game.css";

interface GameProps {
  gameId: string;
  gameState: GameState;
  mark: Mark;
  attemptPlay: (play: Play) => Promise<void>;
}

function Game({ gameId, gameState, attemptPlay, mark }: GameProps) {
  const [error, setError] = useState("");

  const attemptPlayHandler = (row: number, column: number) => () => {
    attemptPlay({
      coord: {
        column,
        row,
      },
      mark,
    }).catch((e) => {
      console.error(e);
      setError("Error Making Play");
    });
  };

  const getColumn = (column: (Mark | undefined)[], rowNum: number) =>
    column.map((mark, colNum) => (
      <MarkComponent key={`${rowNum}${colNum}`} mark={mark} attemptPlay={attemptPlayHandler(rowNum, colNum)} />
    ));

  const rows = gameState.grid.map((row, rowNum) => getColumn(row, rowNum));

  return (
    <div className="Game">
      <div className="grid">{rows}</div>
      <span>{error}</span>
    </div>
  );
}

export default Game;
