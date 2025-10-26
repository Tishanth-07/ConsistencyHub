import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TestService {
  private apiUrl = 'http://localhost:5148/api/Test'; 
  constructor(private http: HttpClient) {}

  getTestData(): Observable<any> {
    console.log('Fetching data from:', this.apiUrl);
    return new Observable(observer => {
      this.http.get<any>(this.apiUrl, { observe: 'response' }).subscribe({
        next: (response) => {
          console.log('Response status:', response.status);
          console.log('Response headers:', response.headers);
          console.log('Response body:', response.body);
          observer.next(response.body);
          observer.complete();
        },
        error: (error) => {
          console.error('HTTP Error:', {
            status: error.status,
            statusText: error.statusText,
            url: error.url,
            error: error.error
          });
          observer.error(error);
        }
      });
    });
  }
}
