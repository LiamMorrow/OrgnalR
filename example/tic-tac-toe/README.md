# OrgnalR Tic Tac Toe

This example shows a multiplayer Tic Tac Toe implemented through SignalR, and Orleans, using OrgnalR to dispatch messages from inside of grains to connected clients.

## Key Example Points

### HubContext Use From Inside Grain

Inside of the GameGrain, after a play has been made in the `AttemptPlayAsync` method, you can see the grain loops through some GameStateNotifiers.
One of the implementations of these notifiers is the `OrgnalRGameHubGameStateNotifier`. This notifier dispatches a message to all connected SignalR clients, letting them know that there is a new game state available using OrgnalR.

## Running

To run this example, there are three apps which need to run at the same time. In different terminals, execute these commands to get them up and running. It is important to run the Silo before the SignalR server, otherwise it won't start up.

### tictactoe-frontend

```
cd tictactoe-frontend
yarn install
yarn start
```

### TicTacToe.OrleansSilo

```
cd TicTacToe.OrleansSilo
dotnet run
```

### TicTacToe.SignalRServer

```
cd TicTacToe.SignalRServer
dotnet run
```
