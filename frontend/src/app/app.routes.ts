import { Routes } from '@angular/router';
import { TestComponent } from './components/test/test';

export const routes: Routes = [
  { path: 'test', component: TestComponent },
  { path: '', redirectTo: '/test', pathMatch: 'full' },
 
];
