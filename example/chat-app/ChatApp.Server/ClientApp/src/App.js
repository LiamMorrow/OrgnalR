import React, { useState } from 'react';
import { Layout } from './components/Layout';
import { Chat } from './components/Chat'
import { Home } from './components/Home'


const App = (props) => {
  const [state, setState] = useState({
    currentChat: undefined,
    chatMessages: []
  })
  props.onReceiveMessage((message) => setState({ ...state, chatMessages: [...state.chatMessages, message] }))
  const joinChat = (chatName) => props.joinChat(chatName).then((messages) => setState({ currentChat: chatName, chatMessages: messages }))
  return (
    <Layout>
      {state.currentChat ? <Chat chatName={state.currentChat} chatMessages={state.chatMessages} sendMessage={props.sendMessage} /> : <Home joinChat={joinChat}></Home>}
    </Layout>
  );
}

export default App
