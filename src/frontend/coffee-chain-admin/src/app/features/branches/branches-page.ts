import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { BranchSummary } from '../../core/models/dashboard.models';
import { BranchApi } from '../../core/services/branch.api';

// BranchesPage hien thi danh sach chi nhanh va KPI co ban tu backend.
@Component({
  selector: 'ccm-branches-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './branches-page.html',
  styleUrl: './branches-page.css'
})
export class BranchesPage {
  private readonly branchApi = inject(BranchApi);

  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly branches = signal<BranchSummary[]>([]);

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.branchApi.getAll().subscribe({
      next: (branches) => {
        this.branches.set(branches);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Khong tai duoc danh sach chi nhanh.');
        this.loading.set(false);
      }
    });
  }
}
