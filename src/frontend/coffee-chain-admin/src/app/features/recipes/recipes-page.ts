import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { RecipeApi, RecipeDto, UpsertRecipeRequestDto, UpsertRecipeIngredientDto } from '../../core/services/recipe.api';
import { InventoryApi } from '../../core/services/inventory.api';
import { LookupItem } from '../../core/models/common.models';

@Component({
  selector: 'ccm-recipes-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './recipes-page.html',
  styleUrl: './recipes-page.css'
})
export class RecipesPage implements OnInit {
  private readonly recipeApi = inject(RecipeApi);
  private readonly inventoryApi = inject(InventoryApi);
  private readonly route = inject(ActivatedRoute);

  protected readonly loading = signal(true);
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly success = signal<string | null>(null);
  
  protected readonly productId = signal<string>('');
  protected readonly productName = signal<string>('Sản phẩm');
  protected readonly instructions = signal<string>('');
  protected readonly ingredients = signal<UpsertRecipeIngredientDto[]>([]);
  
  protected readonly lookupIngredients = signal<LookupItem[]>([]);

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('productId');
      if (id) {
        this.productId.set(id);
        this.load();
      }
    });
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);

    this.inventoryApi.getIngredientLookups().subscribe({
      next: (lookups) => this.lookupIngredients.set(lookups)
    });

    this.recipeApi.getByProductId(this.productId()).subscribe({
      next: (recipe) => {
        this.productName.set(recipe.productName);
        this.instructions.set(recipe.instructions);
        this.ingredients.set(recipe.ingredients.map(i => ({
          ingredientId: i.ingredientId,
          requiredQuantity: i.requiredQuantity
        })));
        this.loading.set(false);
      },
      error: (err) => {
        if (err.status === 404) {
          // No recipe yet, that's fine
          this.ingredients.set([]);
          this.instructions.set('');
        } else {
          this.error.set('Không thể tải công thức.');
        }
        this.loading.set(false);
      }
    });
  }

  protected addIngredient(): void {
    this.ingredients.update(list => [...list, { ingredientId: '', requiredQuantity: 1 }]);
  }

  protected removeIngredient(index: number): void {
    this.ingredients.update(list => list.filter((_, i) => i !== index));
  }

  protected getIngredientName(id: string): string {
    return this.lookupIngredients().find(i => i.id === id)?.label ?? id;
  }

  protected save(): void {
    this.error.set(null);
    this.success.set(null);
    
    // Validate
    const currentIngredients = this.ingredients();
    if (currentIngredients.some(i => !i.ingredientId || i.requiredQuantity <= 0)) {
      this.error.set('Vui lòng điền đủ nguyên liệu và số lượng > 0.');
      return;
    }

    this.submitting.set(true);
    const request: UpsertRecipeRequestDto = {
      productId: this.productId(),
      instructions: this.instructions(),
      ingredients: currentIngredients
    };

    this.recipeApi.upsert(request).subscribe({
      next: () => {
        this.success.set('Đã lưu công thức thành công!');
        this.submitting.set(false);
      },
      error: () => {
        this.error.set('Có lỗi xảy ra khi lưu công thức.');
        this.submitting.set(false);
      }
    });
  }
}
