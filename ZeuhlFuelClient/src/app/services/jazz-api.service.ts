import { inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { forkJoin } from 'rxjs';

// Interface contracts
export interface ChordDto { name: string; notes: string[]; }
export interface harmonizationGroup { targetDegree: number; chords: ChordDto[]; }
export interface dominantHarmonizationGroup { targetDegree: string; chords: ChordDto[]; }

@Injectable({ providedIn: 'root' })
export class JazzApiService {
  private http = inject(HttpClient);

  // Writable signals for storing API responses
  private _nonDomData = signal<harmonizationGroup[]>([]);
  private _domData = signal<dominantHarmonizationGroup[]>([]);

  // Readonly signals for components to subscribe to
  nonDomData = this._nonDomData.asReadonly();
  domData = this._domData.asReadonly();

  fetchHaronizationMatrix(melodyNote: string, nonDomDegrees: number[], domDegrees: string[]): void {
    // Nondominant request
    let nonDomParams = new HttpParams().set('melodyNote', melodyNote);
    nonDomDegrees.forEach(d => nonDomParams = nonDomParams.append('degrees', d.toString()));
    const nonDomRequest = this.http.get<harmonizationGroup[]>('/api/jazz/harmonize', { params: nonDomParams });

    // Dominant request
    let domParams = new HttpParams().set('melodyNote', melodyNote);
    domDegrees.forEach(d => domParams = domParams.append('targetDegrees', d));
    const domRequest = this.http.get<dominantHarmonizationGroup[]>('/api/jazz/harmonize-dominant', { params: domParams });

    // Fire both requests in parallel and update signals when they complete
    forkJoin({
      nonDominant: nonDomRequest,
      dominant: domRequest
    }).subscribe({
      next: (response) => {
        // Only update arrays if user actually requested degrees, otherwise leave them as empty arrays
        this._nonDomData.set(nonDomDegrees.length ? response.nonDominant : []);
        this._domData.set(domDegrees.length ? response.dominant : []);
      },
      error: (err) => console.error('Matrix calculation failed', err)
    });
  };
}
