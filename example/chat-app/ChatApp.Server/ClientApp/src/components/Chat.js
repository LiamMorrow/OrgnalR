import React, { useState } from 'react';

export const Chat = ({ chatName, chatMessages, sendMessage }) => {
  const [senderName, setSenderName] = useState('Some Cool Person')
  const [message, setMessage] = useState('')
  const handleSenderChange = (e) => setSenderName(e.target.value);
  const handleMessageChange = (e) => setMessage(e.target.value);
  return (
    <div>
      <h1>You're in Chat Room: {chatName}</h1>
      {chatMessages.map(message => (<p>{message.senderName}: {message.message}</p>))}
      <label htmlFor="name">Your Name:</label>
      <input name='name' required value={senderName} onChange={handleSenderChange} type='text' />
      <label htmlFor="message">Message</label>
      <input name='message' required value={message} onChange={handleMessageChange} type='text' />
      <button onClick={() => sendMessage(chatName, senderName, message)}>Send</button>
    </div>
  )
};
