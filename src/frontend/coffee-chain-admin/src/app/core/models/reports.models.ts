// reports.models.ts map bao cao doanh thu va cac bang tong hop tu backend.
export interface ReportDailyRevenue {
  date: string;
  revenue: number;
  orders: number;
}

export interface ReportBranchRevenue {
  branchId: string;
  branchName: string;
  revenue: number;
  orders: number;
}

export interface ReportProductRevenue {
  productId: string | null;
  productName: string;
  quantity: number;
  revenue: number;
}

export interface SalesReport {
  fromDate: string | null;
  toDate: string | null;
  branchId: string | null;
  branchName: string | null;
  totalRevenue: number;
  inventoryExpense: number;
  netRevenue: number;
  totalOrders: number;
  averageOrderValue: number;
  activeBranches: number;
  lowStockItems: number;
  activePromotions: number;
  pendingRecruitmentRequests: number;
  dailyRevenue: ReportDailyRevenue[];
  branchRevenue: ReportBranchRevenue[];
  productRevenue: ReportProductRevenue[];
}
