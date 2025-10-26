import { TestBed } from '@angular/core/testing';

import { TestComponent } from '../components/test/test';

describe('Test', () => {
  let service: TestComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TestComponent);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
