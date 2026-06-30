import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { ProductSummary, UpsertProductRequest } from '../models/product.models';
import { PagedResult } from '../models/paged.models';

export interface ProductQuery {
  page: number;
  pageSize: number;
  search?: string | null;
  category?: string | null;
  isAvailable?: boolean | null;
}

// ProductApi doc menu san pham tu backend.
@Injectable({ providedIn: 'root' })
export class ProductApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/products`;

  getAll() {
    return this.http.get<ProductSummary[]>(this.baseUrl);
  }

  getPaged(query: ProductQuery) {
    const params: Record<string, string> = {
      page: String(query.page),
      pageSize: String(query.pageSize)
    };

    if (query.search?.trim()) {
      params['search'] = query.search.trim();
    }

    if (query.category?.trim()) {
      params['category'] = query.category.trim();
    }

    if (query.isAvailable !== null && query.isAvailable !== undefined) {
      params['isAvailable'] = String(query.isAvailable);
    }

    return this.http.get<PagedResult<ProductSummary>>(`${this.baseUrl}/paged`, { params });
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
