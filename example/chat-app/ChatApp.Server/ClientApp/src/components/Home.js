import React, { useState } from 'react';

export const Home = ({ joinChat }) => {

  const [chatName, setChatName] = useState('Some Chat');
  const handleChange = (e) => setChatName(e.target.value);

  const handleJoin = () => joinChat(chatName)
  return (
    <div>
      <h1>G'day! Please join a chat!</h1>
      <label htmlFor="chat-name">Chat Name:</label>
      <input name='chat-name' required value={chatName} onChange={handleChange} type='text' />
      <button onClick={handleJoin}>Join</button>
    </div>
  );
}
