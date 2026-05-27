import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
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

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.promotionApi.getAll().subscribe({
      next: (promotions) => {
        this.promotions.set(promotions);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Khong tai duoc danh sach khuyen mai.');
        this.loading.set(false);
      }
    });
  }

  protected startCreate(): void {
    this.editingId.set(null);
    this.form.set({ name: '', discountPercent: 0, startDate: '', endDate: '', isActive: true });
  }

  protected startEdit(promotion: Promotion): void {
    this.editingId.set(promotion.id);
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
        this.error.set(error?.error?.message ?? 'Khong luu duoc khuyen mai.');
      }
    });
  }

  protected delete(promotion: Promotion): void {
    if (!confirm(`Xoa khuyen mai ${promotion.name}?`)) {
      return;
    }

    this.promotionApi.delete(promotion.id).subscribe({
      next: () => this.load(),
      error: (error) => this.error.set(error?.error?.message ?? 'Khong xoa duoc khuyen mai.')
    });
  }

  protected canWrite(): boolean {
    return this.authStore.can('promotions.write');
  }

  private validate(payload: UpsertPromotionRequest): string | null {
    if (!payload.name.trim()) {
      return 'Ten khuyen mai la bat buoc.';
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
