import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

// PromotionsPage dat san trang khuyen mai de mo rong voi CRUD sau.
@Component({
  selector: 'ccm-promotions-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './promotions-page.html',
  styleUrl: './promotions-page.css'
})
export class PromotionsPage {
  protected readonly promotions = [
    { name: 'Happy Monday', discount: '15%', range: 'Thu 2 hang tuan', status: 'Active' },
    { name: 'Combo Latte + Cake', discount: '10%', range: '01/06 - 30/06', status: 'Draft' },
    { name: 'Member Gold Week', discount: '20%', range: '15/06 - 22/06', status: 'Planned' }
  ];
}
