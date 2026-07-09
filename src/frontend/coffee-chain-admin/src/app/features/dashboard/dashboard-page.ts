import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';

import { ProductSummary } from '../../core/models/dashboard.models';
import { DashboardStore } from './dashboard.store';

@Component({
  selector: 'ccm-dashboard-page',
  standalone: true,
  imports: [CommonModule],
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

  protected productImageSrc(product: ProductSummary): string {
    return product.imageUrl?.trim() || this.fallbackImage(product.category);
  }

  protected useFallbackImage(event: Event, category: string): void {
    const image = event.target as HTMLImageElement;
    if (image.dataset['fallbackApplied']) {
      return;
    }

    image.dataset['fallbackApplied'] = 'true';
    image.src = this.fallbackImage(category);
  }

  private fallbackImage(category: string): string {
    const normalized = category.trim().toLowerCase();
    if (normalized.includes('milk tea')) return 'assets/drinks/milk-tea.svg';
    if (normalized.includes('coffee')) return 'assets/drinks/coffee.svg';
    if (normalized.includes('tea')) return 'assets/drinks/tea.svg';
    if (normalized.includes('juice')) return 'assets/drinks/juice.svg';
    if (normalized.includes('smoothie')) return 'assets/drinks/smoothie.svg';
    if (normalized.includes('ice')) return 'assets/drinks/ice-blended.svg';
    return 'assets/drinks/placeholder.svg';
  }
}
