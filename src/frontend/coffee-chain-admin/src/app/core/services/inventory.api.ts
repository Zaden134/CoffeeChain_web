import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { environment } from '../../../environments/environment';
import { LookupItem } from '../models/common.models';
import { InventoryItem, UpsertInventoryItemRequest } from '../models/inventory.models';

// InventoryApi goi CRUD ton kho va lookup ingredient.
@Injectable({ providedIn: 'root' })
export class InventoryApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/inventory`;

  getAll() {
    return this.http.get<InventoryItem[]>(this.baseUrl);
  }

  getIngredientLookups() {
    return this.http.get<LookupItem[]>(`${this.baseUrl}/ingredients`);
  }

  create(payload: UpsertInventoryItemRequest) {
    return this.http.post<InventoryItem>(this.baseUrl, payload);
  }

  update(id: string, payload: UpsertInventoryItemRequest) {
    return this.http.put<InventoryItem>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
