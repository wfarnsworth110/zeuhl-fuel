import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface ApiResponse {
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class JazzApiService {
  private http = inject(HttpClient);

  // Writable signal managed internally by the service
  private _apiMessage = signal<string>('Awaiting telemetry...');

  // Exposed as a read-only signal so components can watch it but not corrupt it
  public apiMessage = this._apiMessage.asReadonly();

  fetchBackendStatus(): void {
    // Note: The hardcoded localhost URL is GONE. We use a relative path now!
    this.http.get<ApiResponse>('/api/hello').subscribe({
      next: (data) => this._apiMessage.set(data.message),
      error: (err) => {
        this._apiMessage.set('Signal lost.');
        console.error(err);
      }
    });
  }
}
