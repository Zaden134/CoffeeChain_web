// promotion.models.ts map CRUD khuyen mai tu backend.
export interface Promotion {
  id: string;
  name: string;
  discountPercent: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export interface UpsertPromotionRequest {
  name: string;
  discountPercent: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
}
