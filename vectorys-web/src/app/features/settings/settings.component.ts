import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../core/auth/auth.service';
import {
  SettingsPreferencesService,
  UserPreferences
} from '../../core/services/settings-preferences.service';
import { InventaireApiService } from '../../core/services/inventaire-api.service';
import { Inventaire } from '../../shared/models/inventaire.model';
import { environment } from '../../../environments/environment';

type SettingsTab = 'compte' | 'preferences' | 'apropos';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, MatIconModule, MatButtonModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly prefsService = inject(SettingsPreferencesService);
  private readonly inventaireApi = inject(InventaireApiService);
  private readonly router = inject(Router);

  readonly user = this.auth.getCurrentUser();
  activeTab: SettingsTab = 'compte';
  prefs: UserPreferences = this.prefsService.load();
  inventaires: Inventaire[] = [];
  saveMessage = '';
  saveError = '';

  get roleLabel(): string {
    const role = this.user?.role?.toUpperCase() ?? '';
    if (role === 'SUPERVISEUR') return 'Superviseur';
    if (role === 'AGENT') return 'Agent terrain';
    return this.user?.role ?? '—';
  }

  get initials(): string {
    const nom = (this.user?.nom ?? 'S').trim();
    const parts = nom.split(/\s+/).filter(Boolean);
    if (parts.length >= 2) {
      return (parts[0][0] + parts[1][0]).toUpperCase();
    }
    return nom.slice(0, 2).toUpperCase();
  }

  get apiUrl(): string {
    return environment.apiUrl;
  }

  get signalRUrl(): string {
    return environment.signalRUrl;
  }

  get appVersion(): string {
    return '1.0.0';
  }

  get environmentLabel(): string {
    return environment.production ? 'Production' : 'Développement';
  }

  ngOnInit(): void {
    if (!this.auth.isLoggedIn()) {
      void this.router.navigateByUrl('/login');
      return;
    }
    this.inventaireApi.getInventaires(1, 50).subscribe({
      next: (res) => {
        this.inventaires = res.items ?? [];
      },
      error: () => {
        this.inventaires = [];
      }
    });
  }

  setTab(tab: SettingsTab): void {
    this.activeTab = tab;
    this.saveMessage = '';
    this.saveError = '';
  }

  savePreferences(): void {
    this.saveMessage = '';
    this.saveError = '';
    try {
      const inventaireId = this.prefs.inventaireDefautId;
      this.prefs.inventaireDefautId =
        inventaireId === null || inventaireId === undefined || Number.isNaN(Number(inventaireId))
          ? null
          : Number(inventaireId);
      this.prefsService.save(this.prefs);
      this.saveMessage = 'Préférences enregistrées sur cet appareil.';
    } catch {
      this.saveError = "Impossible d'enregistrer les préférences.";
    }
  }

  resetPreferences(): void {
    this.prefs = {
      notificationsEcarts: true,
      notificationsFinEmplacement: true,
      notificationsSon: false,
      inventaireDefautId: null,
      langue: 'fr'
    };
    this.prefsService.save(this.prefs);
    this.saveMessage = 'Préférences réinitialisées.';
    this.saveError = '';
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
