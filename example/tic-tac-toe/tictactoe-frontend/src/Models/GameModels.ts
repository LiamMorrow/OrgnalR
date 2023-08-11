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

export interface ConnectedOpponent {
  isBot: boolean;
}

export interface ConnectedGame {
  state: GameState;
  mark: Mark;
  opponent: ConnectedOpponent;
}
