import { Component, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

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

    connection.on('Send', players => {
      console.log(players);
    });

  }
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
}

export interface Player {
  team: number;
  name: string;
  race: number;
  id: number;
  playerStats: PlayerStats;
  isMe: boolean;
}

export enum Team {
  TeamOne = 0,
  TeamTwo = 1
}

export enum Race {
  Human = 0x01,
  Orc = 0x02,
  NightElf = 0x04,
  Undead = 0x08,
  Random = 0x20
}