import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { ProductSummary } from '../models/dashboard.models';

// ProductApi doc menu san pham tu backend.
@Injectable({ providedIn: 'root' })
export class ProductApi {
  private readonly http = inject(HttpClient);

  getAll() {
    return this.http.get<ProductSummary[]>(`${environment.apiBaseUrl}/products`);
  }
}
