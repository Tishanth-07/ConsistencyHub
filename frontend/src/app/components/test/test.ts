import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { TestService } from '../../services/test';
@Component({
  selector: 'app-test',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './test.html',
  styleUrls: ['./test.css'],
})
export class TestComponent implements OnInit {
  testData: any[] = [];

  constructor(private testService: TestService) {}

  errorMessage: string = '';
  loading: boolean = false;

  ngOnInit(): void {
    this.loading = true;
    this.testService.getTestData().subscribe({
      next: (data) => {
        console.log('Received data:', data);
        this.testData = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching data:', err);
        this.errorMessage = `Error: ${err.message || 'Unknown error occurred'}`;
        this.loading = false;
      }
    });
  }
}
