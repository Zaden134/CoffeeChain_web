import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { EmployeeSummary, UpsertEmployeeRequest } from '../models/employee.models';

// EmployeeApi goi CRUD nhan vien.
@Injectable({ providedIn: 'root' })
export class EmployeeApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/employees`;

  getAll() {
    return this.http.get<EmployeeSummary[]>(this.baseUrl);
  }

  create(payload: UpsertEmployeeRequest) {
    return this.http.post<EmployeeSummary>(this.baseUrl, payload);
  }

  update(id: string, payload: UpsertEmployeeRequest) {
    return this.http.put<EmployeeSummary>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
