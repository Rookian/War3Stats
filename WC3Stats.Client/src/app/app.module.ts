import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { TeamComponent } from "./team/team.component";
import { LoadingComponent } from "./loading/loading.component";
import { QRCodeModule } from "angularx-qrcode";

@NgModule({
  declarations: [
    AppComponent,
    TeamComponent,
    LoadingComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    QRCodeModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
