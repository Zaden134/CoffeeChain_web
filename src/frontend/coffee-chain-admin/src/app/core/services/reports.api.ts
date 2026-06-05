import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { SalesReport } from '../models/reports.models';

export interface SalesReportQuery {
  fromDate?: string | null;
  toDate?: string | null;
  branchId?: string | null;
}

// ReportsApi goi endpoint bao cao va export file tu backend.
@Injectable({ providedIn: 'root' })
export class ReportsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/reports`;

  getSalesReport(query: SalesReportQuery) {
    return this.http.get<SalesReport>(`${this.baseUrl}/sales`, { params: this.toParams(query) });
  }

  exportSalesReport(query: SalesReportQuery, format: 'xlsx' | 'pdf') {
    const params = this.toParams(query).set('format', format);

    return this.http.get(`${this.baseUrl}/sales/export`, {
      params,
      responseType: 'blob',
      observe: 'response'
    });
  }

  private toParams(query: SalesReportQuery): HttpParams {
    let params = new HttpParams();

    if (query.fromDate) {
      params = params.set('fromDate', query.fromDate);
    }

    if (query.toDate) {
      params = params.set('toDate', query.toDate);
    }

    if (query.branchId) {
      params = params.set('branchId', query.branchId);
    }

    return params;
  }
}
