import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { Promotion, UpsertPromotionRequest } from '../models/promotion.models';

// PromotionApi goi CRUD khuyen mai.
@Injectable({ providedIn: 'root' })
export class PromotionApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/promotions`;

  getAll() {
    return this.http.get<Promotion[]>(this.baseUrl);
  }

  create(payload: UpsertPromotionRequest) {
    return this.http.post<Promotion>(this.baseUrl, payload);
  }

  update(id: string, payload: UpsertPromotionRequest) {
    return this.http.put<Promotion>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
