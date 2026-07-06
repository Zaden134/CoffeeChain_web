import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';

import { DashboardStore } from './dashboard.store';

@Component({
  selector: 'ccm-dashboard-page',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.css',
  providers: [DashboardStore]
})
export class DashboardPage {
  protected readonly store = inject(DashboardStore);
  protected readonly state = this.store.state;

  protected readonly maxBranchRevenue = computed(() => {
    const branches = this.state().data?.topBranches ?? [];
    return Math.max(...branches.map((branch) => branch.revenueToday), 1);
  });

  protected branchRevenuePercent(revenue: number): number {
    return Math.max(4, Math.round((revenue / this.maxBranchRevenue()) * 100));
  }
}
