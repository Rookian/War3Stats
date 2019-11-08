import { Component, OnInit, Input } from "@angular/core";
import { Player } from "../app.component";

@Component({
  selector: "app-team",
  templateUrl: "./team.component.html",
  styleUrls: ["./team.component.css"]
})
export class TeamComponent implements OnInit {

  @Input() team: Player[];
  constructor() { }

  ngOnInit() {
  }

}
