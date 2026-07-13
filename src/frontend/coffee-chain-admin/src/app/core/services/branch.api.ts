import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { BranchSummary } from '../models/dashboard.models';

export interface UpsertBranchRequest {
  code: string;
  name: string;
  address: string;
  managerName: string;
  isActive: boolean;
}

// BranchApi doc danh sach chi nhanh tu backend.
@Injectable({ providedIn: 'root' })
export class BranchApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/branches`;

  getAll() {
    return this.http.get<BranchSummary[]>(this.baseUrl);
  }

  update(id: string, payload: UpsertBranchRequest) {
    return this.http.put<BranchSummary>(`${this.baseUrl}/${id}`, payload);
  }
}
