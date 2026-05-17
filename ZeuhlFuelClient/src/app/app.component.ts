import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { JazzApiService } from './services/jazz-api.service';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'Zeuhl-Fuel Reharm Matrix';
  private jazzApi = inject(JazzApiService);

  // Hook up dual signals
  nonDomResults = this.jazzApi.nonDomData;
  domResults = this.jazzApi.domData;

  availableNotes = ['C', 'Db', 'D', 'Eb', 'E', 'F', 'Gb', 'G', 'Ab', 'A', 'Bb', 'B'];
  availableNonDom = [1, 3, 5, 7, 9, 11, 13];
  availableDom = ['1', 'b9', '9', '#9', '3', 'b11', '#11', '5', 'b13', '13', '7'];

  // Default selections
  selectedNote = 'C';
  selectedNonDom: number[] = [3, 7];
  selectedDom: string[] = ['#9', 'b13'];

  calculateMatrix(): void {
    if (this.selectedNonDom.length === 0 && this.selectedDom.length === 0) {
      return;
    }
    this.jazzApi.fetchHaronizationMatrix(this.selectedNote, this.selectedNonDom, this.selectedDom);
  }

  // Number array toggler
  toggleNonDom(degree: number, isChecked: boolean): void {
    if (isChecked) this.selectedNonDom = [...this.selectedNonDom, degree].sort((a, b) => a - b);
    else this.selectedNonDom = this.selectedNonDom.filter(d => d !== degree);
  }

  // String array toggler
  toggleDom(degree: string, isChecked: boolean): void {
    if (isChecked) this.selectedDom = [...this.selectedDom, degree];
    else this.selectedDom = this.selectedDom.filter(d => d !== degree);
  }

  ngOnInit(): void {
    this.calculateMatrix();
  }
}
