import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface InventoryTransactionDto {
  id: string;
  ingredientId: string;
  ingredientName: string;
  branchId: string;
  branchName: string;
  type: number; // 1 = Import, 2 = Export, 3 = Adjustment
  quantity: number;
  unitCost: number;
  transactionAmount: number;
  referenceNumber: string;
  notes: string;
  createdBy: string;
  createdAtUtc: string;
}

export interface CreateInventoryTransactionRequestDto {
  ingredientId: string;
  branchId: string;
  type: number;
  quantity: number;
  unitCost: number;
  referenceNumber: string;
  notes: string;
}

@Injectable({ providedIn: 'root' })
export class InventoryTransactionApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/inventory-transactions`;

  getAll(): Observable<InventoryTransactionDto[]> {
    return this.http.get<InventoryTransactionDto[]>(this.baseUrl);
  }

  create(request: CreateInventoryTransactionRequestDto): Observable<InventoryTransactionDto> {
    return this.http.post<InventoryTransactionDto>(this.baseUrl, request);
  }
}
