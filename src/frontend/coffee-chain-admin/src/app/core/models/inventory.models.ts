// inventory.models.ts map CRUD ton kho tu backend.
export interface InventoryItem {
  id: string;
  branchId: string;
  branchName: string;
  ingredientId: string;
  ingredientName: string;
  unit: string;
  reorderLevel: number;
  inStockQuantity: number;
  reservedQuantity: number;
  isLowStock: boolean;
}

export interface UpsertInventoryItemRequest {
  ingredientId: string | null;
  ingredientName?: string | null;
  unit?: string | null;
  reorderLevel: number;
  branchId: string;
  inStockQuantity: number;
  reservedQuantity: number;
}
