export type Mark = "X" | "O";

export interface Coord {
  row: number;
  column: number;
}

export interface Play {
  mark: Mark;
  coord: Coord;
}

export interface GameState {
  turn: Mark;
  grid: Mark[][];
  winner: Mark | undefined;
  draw: boolean;
}
