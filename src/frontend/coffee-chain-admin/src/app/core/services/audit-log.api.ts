import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { AuditLogEntry, AuditLogQuery } from '../models/audit.models';
import { PagedResult } from '../models/paged.models';

@Injectable({ providedIn: 'root' })
export class AuditLogApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/audit-logs`;

  getPaged(query: AuditLogQuery) {
    let params = new HttpParams()
      .set('page', query.page)
      .set('pageSize', query.pageSize);

    if (query.search?.trim()) {
      params = params.set('search', query.search.trim());
    }

    return this.http.get<PagedResult<AuditLogEntry>>(this.baseUrl, { params });
  }
}
