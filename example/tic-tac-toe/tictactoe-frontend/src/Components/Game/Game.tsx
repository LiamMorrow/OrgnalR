import { useState } from "react";
import { GameState, Play, Symbl } from "../../Models/GameModels";
import "./Game.css";

interface GameProps {
  gameId: string;
  gameState: GameState;
  symbol: Symbl;
  attemptPlay: (play: Play) => Promise<void>;
}

const SymblComponent = ({
  symbl,
  row,
  col,
  attemptPlay,
}: {
  symbl: Symbl | undefined;
  row: number;
  col: number;
  attemptPlay: () => void;
}) => (
  <button onClick={attemptPlay}>
    row:{row} col: {col}
    {symbl}
  </button>
);

function Game({ gameId, gameState, attemptPlay, symbol }: GameProps) {
  const [error, setError] = useState("");

  const attemptPlayHandler = (row: number, column: number) => () => {
    attemptPlay({
      coord: {
        column,
        row,
      },
      symbol,
    }).catch((e) => {
      console.error(e);
      setError("Error Making Play");
    });
  };

  const getColumn = (column: (Symbl | undefined)[], rowNum: number) =>
    column.map((smb, colNum) => (
      <SymblComponent
        key={rowNum + "" + colNum}
        symbl={smb}
        row={rowNum}
        col={colNum}
        attemptPlay={attemptPlayHandler(rowNum, colNum)}
      />
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
