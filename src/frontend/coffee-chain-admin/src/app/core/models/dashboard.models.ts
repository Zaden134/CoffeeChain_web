// Cac model nay map truc tiep voi response tu ASP.NET API de frontend type-safe.
export interface KpiSummary {
  dailyRevenue: number;
  monthlyRevenue: number;
  totalOrders: number;
  activeBranches: number;
  lowStockAlerts: number;
}

export interface BranchSummary {
  id: string;
  code: string;
  name: string;
  address: string;
  managerName: string;
  revenueToday: number;
  lowStockItems: number;
  isActive: boolean;
}

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

export interface DashboardOverview {
  summary: KpiSummary;
  topBranches: BranchSummary[];
  bestSellingProducts: ProductSummary[];
}
