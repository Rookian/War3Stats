import { Component, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { inherits } from 'util';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  teamOne: Player[] = [];
  teamTwo: Player[] = [];
  ngOnInit(): void {
    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl('https://localhost:5001/wc3', {
        transport: signalR.HttpTransportType.WebSockets,
        skipNegotiation: true
      })
      .build();

    connection.start().then(() => {
      console.log('SignalR Connected!');
    }).catch(err => {
      return console.error(err);
    });

    connection.on('Send', result => {
      const players = result as Player[];

      this.teamOne = players.filter(x => x.team === 'TeamOne').sort((a, b) => a.playerStats.winRate > b.playerStats.winRate ? -1 : 1);
      this.teamTwo = players.filter(x => x.team === 'TeamTwo').sort((a, b) => a.playerStats.winRate > b.playerStats.winRate ? -1 : 1);

      console.log(players);
    });
  }
}

export interface Grouped<TKey, TValue> extends Array<TValue> {
  key: TKey;
}

export interface PlayerStats {
  soloWins: number;
  soloLosses: number;
  soloGames: number;
  soloWinRate: number;
  soloLevel: number;
  teamWins: number;
  teamLosses: number;
  teamGames: number;
  teamWinRate: number;
  teamLevel: number;
  winRate: number;
}

export interface Player {
  team: string;
  name: string;
  race: string;
  id: number;
  playerStats: PlayerStats;
  isMe: boolean;
}

export enum Team {
  TeamOne = 0,
  TeamTwo = 1
}

// export enum Race {
//   Human = 'Human',
//   Orc = 'Orc',
//   NightElf = 'NightElf',
//   Undead = 'Undead',
//   Random = 'Random'
// }


