import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { JazzApiService } from './services/jazz-api.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  title = 'ZeuhlFuelClient';
  private jazzApiService = inject(JazzApiService);

  apiMessage = this.jazzApiService.apiMessage;

  ngOnInit(): void {
    this.jazzApiService.fetchBackendStatus();
  }
}
