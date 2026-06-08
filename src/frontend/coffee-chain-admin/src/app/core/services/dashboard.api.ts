import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { DashboardOverview } from '../models/dashboard.models';
import { environment } from '../../../environments/environment';

// DashboardApi tap trung goi endpoint tong quan, giu component sach va de test.
@Injectable({ providedIn: 'root' })
export class DashboardApi {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiBaseUrl}/dashboard/overview`;

  getOverview() {
    return this.http.get<DashboardOverview>(this.endpoint);
  }
}
