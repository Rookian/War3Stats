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
      .withUrl('https://localhost:5001/wc3')
      .build();

    connection.start().then(() => {
      console.log('SignalR Connected!');
    }).catch(err => {
      return console.error(err.toString());
    });

    connection.on('Connected', (conid, message) => {
      console.log(conid + ' ' + message);
    });

  }
}
