import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { BranchSummary } from '../../core/models/dashboard.models';
import { SalesReport } from '../../core/models/reports.models';
import { AuthStore } from '../../core/services/auth.store';
import { BranchApi } from '../../core/services/branch.api';
import { ReportsApi } from '../../core/services/reports.api';

// ReportsPage hien thi bao cao thuc te va cho phep loc/xuat file.
@Component({
  selector: 'ccm-reports-page',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, FormsModule],
  templateUrl: './reports-page.html',
  styleUrl: './reports-page.css'
})
export class ReportsPage {
  private readonly reportsApi = inject(ReportsApi);
  private readonly branchApi = inject(BranchApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly exporting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly report = signal<SalesReport | null>(null);
  protected readonly branches = signal<BranchSummary[]>([]);
  protected readonly query = signal({
    fromDate: '',
    toDate: '',
    branchId: this.authStore.role() === 'Administrator' ? '' : (this.authStore.branchId() ?? '')
  });

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.branchApi.getAll().subscribe({
      next: (branches) => this.branches.set(branches),
      error: () => undefined
    });

    this.reportsApi.getSalesReport(this.query()).subscribe({
      next: (report) => {
        this.report.set(report);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Không tải được báo cáo.');
        this.loading.set(false);
      }
    });
  }

  protected export(format: 'xlsx' | 'pdf'): void {
    this.exporting.set(true);
    this.error.set(null);

    this.reportsApi.exportSalesReport(this.query(), format).subscribe({
      next: (response) => {
        this.exporting.set(false);
        const blob = response.body;
        if (!blob) {
          this.error.set('Không tải được file báo cáo.');
          return;
        }

        const filename = this.extractFilename(response.headers.get('content-disposition'), format);
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = filename;
        anchor.click();
        URL.revokeObjectURL(url);
      },
      error: () => {
        this.exporting.set(false);
        this.error.set('Không xuất được file báo cáo.');
      }
    });
  }

  protected canChangeBranch(): boolean {
    return this.authStore.role() === 'Administrator';
  }

  protected updateQuery(key: 'fromDate' | 'toDate' | 'branchId', value: string): void {
    this.query.update((query) => ({ ...query, [key]: value }));
  }

  private extractFilename(contentDisposition: string | null, format: 'xlsx' | 'pdf'): string {
    const match = contentDisposition?.match(/filename="?([^"]+)"?/i);
    return match?.[1] ?? this.buildExportFilename(format);
  }

  private buildExportFilename(format: 'xlsx' | 'pdf'): string {
    const query = this.query();
    const branchName = this.branches().find((branch) => branch.id === query.branchId)?.name ?? 'toan-he-thong';
    const from = query.fromDate || 'tu-dau';
    const to = query.toDate || new Date().toISOString().slice(0, 10);
    return `bao-cao-doanh-thu_${this.slugify(branchName)}_${from}_${to}.${format}`;
  }

  private slugify(value: string): string {
    return value
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .toLowerCase()
      .replace(/đ/g, 'd')
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/^-+|-+$/g, '');
  }
}
