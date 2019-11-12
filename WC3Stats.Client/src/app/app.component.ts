import { Component, OnInit } from "@angular/core";
import * as signalR from "@aspnet/signalr";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.css"]
})

export class AppComponent implements OnInit {
  teamOne: Player[] = [];
  teamTwo: Player[] = [];
  state: signalR.HubConnectionState;
  states = signalR.HubConnectionState;

  async ngOnInit(): Promise<void> {

    const connection = this.Connect();

    await this.start(connection);

    connection.onclose(async () => {
      console.log("on close");
      this.state = connection.state;
      await this.start(connection);
    });

    connection.on("Send", result => {
      const players = result as Player[];
      this.teamOne = players.filter(x => x.team === "TeamOne").sort((a, b) => a.playerStats.winRate > b.playerStats.winRate ? -1 : 1);
      this.teamTwo = players.filter(x => x.team === "TeamTwo").sort((a, b) => a.playerStats.winRate > b.playerStats.winRate ? -1 : 1);
      console.log(players);
    });

    connection.on("GameFound", () => {
      this.teamOne = [];
      this.teamTwo = [];
    });
  }

  private Connect() {
    const url = `${document.location.protocol}//${document.location.host}/wc3`;
    console.log("url", url);
    return new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(url, {
        transport: signalR.HttpTransportType.WebSockets,
        skipNegotiation: true
      })
      .build();
  }

  async start(connection: signalR.HubConnection): Promise<void> {
    try {
      await connection.start();
      this.state = connection.state;
      console.log("connected");
    } catch (err) {
      this.state = connection.state;
      console.log(err);
      setTimeout(() => this.start(connection), 2000);
    }
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
  winRate: number;
}

export interface Player {
  team: string;
  name: string;
  race: Race;
  id: number;
  playerStats: PlayerStats;
  isMe: boolean;
}

export enum Team {
  TeamOne = 0,
  TeamTwo = 1
}

export enum Race {
  Human = "Human",
  Orc = "Orc",
  NightElf = "NightElf",
  Undead = "Undead",
  Random = "Random"
}
