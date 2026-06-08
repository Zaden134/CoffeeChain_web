import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { BranchSummary } from '../models/dashboard.models';

// BranchApi doc danh sach chi nhanh tu backend.
@Injectable({ providedIn: 'root' })
export class BranchApi {
  private readonly http = inject(HttpClient);

  getAll() {
    return this.http.get<BranchSummary[]>(`${environment.apiBaseUrl}/branches`);
  }
}
