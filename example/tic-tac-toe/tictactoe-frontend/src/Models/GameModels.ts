export type Symbl = "X" | "O";

export interface Coord {
  row: number;
  column: number;
}

export interface Play {
  symbol: Symbl;
  coord: Coord;
}

export interface GameState {
  turn: Symbl;
  grid: Symbl[][];
  winner: Symbl | undefined;
  draw: boolean;
}
