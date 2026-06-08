import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Promotion, UpsertPromotionRequest } from '../../core/models/promotion.models';
import { AuthStore } from '../../core/services/auth.store';
import { PromotionApi } from '../../core/services/promotion.api';

// PromotionsPage noi API CRUD khuyen mai va an hien action theo role.
@Component({
  selector: 'ccm-promotions-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './promotions-page.html',
  styleUrl: './promotions-page.css'
})
export class PromotionsPage {
  private readonly promotionApi = inject(PromotionApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly promotions = signal<Promotion[]>([]);
  protected readonly editingId = signal<string | null>(null);
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);
  protected readonly pageSize = signal(5);

  protected readonly form = signal<UpsertPromotionRequest>({
    name: '',
    discountPercent: 0,
    startDate: '',
    endDate: '',
    isActive: true
  });

  constructor() {
    this.load();
  }

  protected readonly filteredPromotions = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) {
      return this.promotions();
    }

    return this.promotions().filter((promotion) =>
      [promotion.name, promotion.startDate, promotion.endDate, promotion.isActive ? 'active' : 'inactive']
        .join(' ')
        .toLowerCase()
        .includes(term));
  });

  protected readonly pagedPromotions = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filteredPromotions().slice(start, start + this.pageSize());
  });

  protected readonly totalPages = computed(() => {
    const total = this.filteredPromotions().length;
    return Math.max(1, Math.ceil(total / this.pageSize()));
  });

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.promotionApi.getAll().subscribe({
      next: (promotions) => {
        this.promotions.set(promotions);
        this.page.set(1);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Không tải được danh sách khuyến mãi.');
        this.loading.set(false);
      }
    });
  }

  protected startCreate(): void {
    this.editingId.set(null);
    this.page.set(1);
    this.form.set({ name: '', discountPercent: 0, startDate: '', endDate: '', isActive: true });
  }

  protected startEdit(promotion: Promotion): void {
    this.editingId.set(promotion.id);
    this.page.set(1);
    this.form.set({
      name: promotion.name,
      discountPercent: promotion.discountPercent,
      startDate: promotion.startDate,
      endDate: promotion.endDate,
      isActive: promotion.isActive
    });
  }

  protected submit(): void {
    const payload = this.form();
    this.error.set(this.validate(payload));
    if (this.error()) {
      return;
    }

    this.submitting.set(true);
    const request = this.editingId()
      ? this.promotionApi.update(this.editingId()!, payload)
      : this.promotionApi.create(payload);

    request.subscribe({
      next: () => {
        this.submitting.set(false);
        this.startCreate();
        this.load();
      },
      error: (error) => {
        this.submitting.set(false);
        this.error.set(error?.error?.message ?? 'Không lưu được khuyến mãi.');
      }
    });
  }

  protected delete(promotion: Promotion): void {
    if (!confirm(`Xóa khuyến mãi ${promotion.name}?`)) {
      return;
    }

    this.promotionApi.delete(promotion.id).subscribe({
      next: () => this.load(),
      error: (error) => this.error.set(error?.error?.message ?? 'Không xóa được khuyến mãi.')
    });
  }

  protected canWrite(): boolean {
    return this.authStore.can('promotions.write');
  }

  protected nextPage(): void {
    this.page.update((value) => Math.min(value + 1, this.totalPages()));
  }

  protected previousPage(): void {
    this.page.update((value) => Math.max(value - 1, 1));
  }

  protected changeSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  private validate(payload: UpsertPromotionRequest): string | null {
    if (!payload.name.trim()) {
      return 'Tên khuyến mãi là bắt buộc.';
    }

    if (!payload.startDate || !payload.endDate) {
      return 'Ngay bat dau va ket thuc la bat buoc.';
    }

    if (payload.endDate < payload.startDate) {
      return 'Ngay ket thuc khong duoc nho hon ngay bat dau.';
    }

    return null;
  }
}
