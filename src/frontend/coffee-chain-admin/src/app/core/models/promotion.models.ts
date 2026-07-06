// promotion.models.ts map CRUD khuyen mai tu backend.
export interface Promotion {
  id: string;
  code: string;
  name: string;
  discountPercent: number | null;
  discountAmount: number | null;
  startDate: string;
  endDate: string;
  branchId: string | null;
  branchName: string | null;
  customerSegment: string | null;
  customerPhone: string | null;
  isActive: boolean;
}

export interface UpsertPromotionRequest {
  code: string;
  name: string;
  discountPercent: number | null;
  discountAmount: number | null;
  startDate: string;
  endDate: string;
  branchId: string | null;
  customerSegment: string | null;
  customerPhone: string | null;
  isActive: boolean;
}
