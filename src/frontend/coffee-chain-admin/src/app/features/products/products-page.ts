import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { ProductSummary, UpsertProductRequest } from '../../core/models/product.models';
import { PagedResult } from '../../core/models/paged.models';
import { AuthStore } from '../../core/services/auth.store';
import { ProductApi } from '../../core/services/product.api';

@Component({
  selector: 'ccm-products-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './products-page.html',
  styleUrl: './products-page.css'
})
export class ProductsPage {
  private readonly productApi = inject(ProductApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly result = signal<PagedResult<ProductSummary> | null>(null);
  protected readonly editingId = signal<string | null>(null);
  protected readonly searchTerm = signal('');
  protected readonly page = signal(1);
  protected readonly pageSize = signal(8);

  protected readonly form = signal<UpsertProductRequest>({
    sku: '',
    name: '',
    category: '',
    price: 0,
    imageUrl: '',
    isAvailable: true
  });

  constructor() {
    this.load();
  }

  protected products(): ProductSummary[] {
    return this.result()?.items ?? [];
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

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.productApi.getPaged({
      page: this.page(),
      pageSize: this.pageSize(),
      search: this.searchTerm()
    }).subscribe({
      next: (result) => {
        this.result.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Không tải được danh sách sản phẩm.');
        this.loading.set(false);
      }
    });
  }

  protected startCreate(): void {
    this.editingId.set(null);
    this.form.set({
      sku: '',
      name: '',
      category: '',
      price: 0,
      imageUrl: '',
      isAvailable: true
    });
  }

  protected startEdit(product: ProductSummary): void {
    this.editingId.set(product.id);
    this.form.set({
      sku: product.sku,
      name: product.name,
      category: product.category,
      price: product.price,
      imageUrl: product.imageUrl,
      isAvailable: product.isAvailable
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
      ? this.productApi.update(this.editingId()!, payload)
      : this.productApi.create(payload);

    request.subscribe({
      next: () => {
        this.submitting.set(false);
        this.startCreate();
        this.load();
      },
      error: (error) => {
        this.submitting.set(false);
        this.error.set(error?.error?.message ?? 'Không lưu được sản phẩm.');
      }
    });
  }

  protected delete(product: ProductSummary): void {
    if (!confirm(`Ẩn sản phẩm ${product.name}?`)) {
      return;
    }

    this.productApi.delete(product.id).subscribe({
      next: () => this.load(),
      error: (error) => this.error.set(error?.error?.message ?? 'Không ẩn được sản phẩm.')
    });
  }

  protected deleteEditing(): void {
    const product = this.products().find((item) => item.id === this.editingId());
    if (product) {
      this.delete(product);
    }
  }

  protected canWrite(): boolean {
    return this.authStore.role() === 'Administrator' || this.authStore.role() === 'BranchManager';
  }

  protected nextPage(): void {
    this.page.update((value) => Math.min(value + 1, this.result()?.totalPages ?? 1));
    this.load();
  }

  protected previousPage(): void {
    this.page.update((value) => Math.max(value - 1, 1));
    this.load();
  }

  protected changeSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
    this.load();
  }

  private validate(payload: UpsertProductRequest): string | null {
    if (!payload.sku.trim() || !payload.name.trim() || !payload.category.trim()) {
      return 'SKU, tên sản phẩm và danh mục là bắt buộc.';
    }

    if (payload.price <= 0) {
      return 'Giá bán phải lớn hơn 0.';
    }

    return null;
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
