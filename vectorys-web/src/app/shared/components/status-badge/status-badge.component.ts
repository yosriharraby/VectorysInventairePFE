import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `<span class="badge" [class]="cssClass">{{ statut }}</span>`,
  styles: [
    `
      .badge {
        display: inline-block;
        padding: 0.2rem 0.6rem;
        border-radius: 999px;
        font-size: 0.75rem;
        font-weight: 600;
        text-transform: uppercase;
      }
      .prep {
        background: #fef3c7;
        color: #92400e;
      }
      .cours {
        background: #dbeafe;
        color: #1e40af;
      }
      .cloture {
        background: #dcfce7;
        color: #166534;
      }
      .default {
        background: #f1f5f9;
        color: #475569;
      }
    `
  ]
})
export class StatusBadgeComponent {
  @Input({ required: true }) statut!: string;

  get cssClass(): string {
    switch (this.statut?.toUpperCase()) {
      case 'EN_PREPARATION':
        return 'prep';
      case 'EN_COURS':
        return 'cours';
      case 'CLOTURE':
        return 'cloture';
      default:
        return 'default';
    }
  }
}
