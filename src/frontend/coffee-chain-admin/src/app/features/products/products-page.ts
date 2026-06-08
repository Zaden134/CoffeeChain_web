import { CommonModule, CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';

import { ProductSummary } from '../../core/models/dashboard.models';
import { ProductApi } from '../../core/services/product.api';

// ProductsPage doc menu san pham tu backend va hien thi theo card.
@Component({
  selector: 'ccm-products-page',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './products-page.html',
  styleUrl: './products-page.css'
})
export class ProductsPage {
  private readonly productApi = inject(ProductApi);

  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly products = signal<ProductSummary[]>([]);

  constructor() {
    this.load();
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.productApi.getAll().subscribe({
      next: (products) => {
        this.products.set(products);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Không tải được danh sách sản phẩm.');
        this.loading.set(false);
      }
    });
  }
}
