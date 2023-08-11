import { Mark } from "../../Models/GameModels";

interface MarkProps {
  mark: Mark | undefined;
  attemptPlay: () => void;
}

const MarkComponent = ({ mark, attemptPlay }: MarkProps) => <button onClick={attemptPlay}>{mark}</button>;

export default MarkComponent;
