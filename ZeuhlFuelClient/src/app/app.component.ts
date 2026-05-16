import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpClient } from '@angular/common/http';

// Define an interface matching the expected structure of the API response
interface ApiResponse {
  message: string;
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'ZeuhlFuelClient';
  private http = inject(HttpClient);

  apiMessage = signal<string>('Loading...');

  ngOnInit(): void {
    this.http.get<ApiResponse>('http://localhost:5206')
      .subscribe({
        next: (data) => {
          this.apiMessage.set(data.message);
        },
        error: (err) => {
          this.apiMessage.set('Error fetching data from API');
          console.error('API error:', err);
        }
      });
  }
}
