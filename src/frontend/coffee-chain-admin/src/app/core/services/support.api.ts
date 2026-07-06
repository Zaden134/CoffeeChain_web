import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { environment } from '../../../environments/environment';

export interface CreateSupportRequest {
  subject: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class SupportApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/support`;

  create(request: CreateSupportRequest) {
    return this.http.post<{ message: string }>(`${this.baseUrl}/requests`, request);
  }
}
