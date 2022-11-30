import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import * as signalR from '@microsoft/signalr'

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');
const root = createRoot(rootElement);

let connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7280/chat")
  .build();

let onReceiveMessage;
connection.on("NewMessage", data => {
  if (onReceiveMessage) {
    onReceiveMessage(data);
  }
});


connection.start();

const joinChat = (chatName) => connection.invoke("JoinChat", { chatName });
const sendMessage = (chatName, senderName, message) => connection.send("SendMessage", { chatName, senderName, message })
const receiveMessage = (cb) => { onReceiveMessage = cb }

root.render(
  <BrowserRouter basename={baseUrl}>
    <App joinChat={joinChat} sendMessage={sendMessage} onReceiveMessage={receiveMessage} />
  </BrowserRouter>);
