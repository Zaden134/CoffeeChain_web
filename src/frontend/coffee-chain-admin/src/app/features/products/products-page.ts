import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { ProductSummary, UpsertProductRequest } from '../../core/models/product.models';
import { AuthStore } from '../../core/services/auth.store';
import { ProductApi } from '../../core/services/product.api';

// ProductsPage quan ly menu san pham va cong thuc lien quan.
@Component({
  selector: 'ccm-products-page',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, FormsModule, RouterLink],
  templateUrl: './products-page.html',
  styleUrl: './products-page.css'
})
export class ProductsPage {
  private readonly productApi = inject(ProductApi);
  protected readonly authStore = inject(AuthStore);

  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly products = signal<ProductSummary[]>([]);
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

  protected readonly filteredProducts = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    if (!term) {
      return this.products();
    }

    return this.products().filter((product) =>
      [product.sku, product.name, product.category]
        .join(' ')
        .toLowerCase()
        .includes(term));
  });

  protected readonly pagedProducts = computed(() => {
    const start = (this.page() - 1) * this.pageSize();
    return this.filteredProducts().slice(start, start + this.pageSize());
  });

  protected readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.filteredProducts().length / this.pageSize())));

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.productApi.getAll().subscribe({
      next: (products) => {
        this.products.set(products);
        this.page.set(1);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Khong tai duoc danh sach san pham.');
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
        this.error.set(error?.error?.message ?? 'Khong luu duoc san pham.');
      }
    });
  }

  protected delete(product: ProductSummary): void {
    if (!confirm(`An san pham ${product.name}?`)) {
      return;
    }

    this.productApi.delete(product.id).subscribe({
      next: () => this.load(),
      error: (error) => this.error.set(error?.error?.message ?? 'Khong an duoc san pham.')
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
    this.page.update((value) => Math.min(value + 1, this.totalPages()));
  }

  protected previousPage(): void {
    this.page.update((value) => Math.max(value - 1, 1));
  }

  protected changeSearch(value: string): void {
    this.searchTerm.set(value);
    this.page.set(1);
  }

  private validate(payload: UpsertProductRequest): string | null {
    if (!payload.sku.trim() || !payload.name.trim() || !payload.category.trim()) {
      return 'SKU, ten san pham va danh muc la bat buoc.';
    }

    if (payload.price <= 0) {
      return 'Gia ban phai lon hon 0.';
    }

    return null;
  }
}
