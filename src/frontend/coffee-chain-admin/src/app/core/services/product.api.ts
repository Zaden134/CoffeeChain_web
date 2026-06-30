import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { ProductSummary, UpsertProductRequest } from '../models/product.models';

// ProductApi doc menu san pham tu backend.
@Injectable({ providedIn: 'root' })
export class ProductApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/products`;

  getAll() {
    return this.http.get<ProductSummary[]>(this.baseUrl);
  }

  create(payload: UpsertProductRequest) {
    return this.http.post<ProductSummary>(this.baseUrl, payload);
  }

  update(id: string, payload: UpsertProductRequest) {
    return this.http.put<ProductSummary>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
