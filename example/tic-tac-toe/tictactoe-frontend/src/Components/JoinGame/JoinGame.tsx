import { MouseEvent, useState } from "react";
import "./JoinGame.css";

function JoinGame(props: { joinGame: (gameId: string) => Promise<void> }) {
  const [gameId, setGameId] = useState<string>("");
  const [error, setError] = useState<string>("");
  const handleClick = (e: MouseEvent) => {
    e.preventDefault();
    props.joinGame(gameId).catch((e) => {
      console.error(e);
      setError("Cannot Join Game");
    });
  };

  return (
    <form className="JoinGame">
      <label htmlFor="gameId">Game ID:</label>
      <input
        type="text"
        required
        minLength={4}
        autoComplete="off"
        value={gameId}
        onChange={(event) => setGameId(event.target.value)}
      />
      <span className="error">{error}</span>
      <button onClick={(e) => handleClick(e)}>Join</button>
    </form>
  );
}

export default JoinGame;
