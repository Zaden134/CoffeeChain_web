import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

export interface RecipeIngredientDto {
  id: string;
  recipeId: string;
  ingredientId: string;
  ingredientName: string;
  ingredientUnit: string;
  requiredQuantity: number;
}

export interface RecipeDto {
  id: string;
  productId: string;
  productName: string;
  instructions: string;
  ingredients: RecipeIngredientDto[];
}

export interface UpsertRecipeIngredientDto {
  ingredientId: string;
  requiredQuantity: number;
}

export interface UpsertRecipeRequestDto {
  productId: string;
  instructions: string;
  ingredients: UpsertRecipeIngredientDto[];
}

@Injectable({ providedIn: 'root' })
export class RecipeApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/recipes`;

  getAll(): Observable<RecipeDto[]> {
    return this.http.get<RecipeDto[]>(this.baseUrl);
  }

  getByProductId(productId: string): Observable<RecipeDto> {
    return this.http.get<RecipeDto>(`${this.baseUrl}/product/${productId}`);
  }

  upsert(request: UpsertRecipeRequestDto): Observable<RecipeDto> {
    return this.http.post<RecipeDto>(this.baseUrl, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
