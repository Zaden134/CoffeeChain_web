export interface ProductSummary {
  id: string;
  sku: string;
  name: string;
  category: string;
  price: number;
  imageUrl: string;
  isAvailable: boolean;
  soldQuantity: number;
}

export interface UpsertProductRequest {
  sku: string;
  name: string;
  category: string;
  price: number;
  imageUrl: string;
  isAvailable: boolean;
}
