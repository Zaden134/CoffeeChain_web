import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';

import { DashboardStore } from '../dashboard/dashboard.store';

// ReportsPage dung du lieu thuc de tong hop KPI va danh sach can uu tien.
@Component({
  selector: 'ccm-reports-page',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './reports-page.html',
  styleUrl: './reports-page.css'
})
export class ReportsPage {
  protected readonly store = inject(DashboardStore);
  protected readonly state = this.store.state;
  protected readonly reportCards = computed(() => {
    const data = this.state().data;
    if (!data) {
      return [];
    }

    const branchCount = Math.max(data.summary.activeBranches, 1);
    const estimatedWeeklyRevenue = data.summary.dailyRevenue * 7;
    const averageRevenuePerBranch = data.summary.monthlyRevenue / branchCount;
    const lowStockRate = data.summary.lowStockAlerts / branchCount;

    return [
      { label: 'Uoc tinh doanh thu tuan', value: estimatedWeeklyRevenue, kind: 'currency' as const },
      { label: 'Doanh thu thang', value: data.summary.monthlyRevenue, kind: 'currency' as const },
      { label: 'Doanh thu/chi nhanh', value: averageRevenuePerBranch, kind: 'currency' as const },
      { label: 'Canh bao kho/chi nhanh', value: lowStockRate, kind: 'number' as const }
    ];
  });
}
