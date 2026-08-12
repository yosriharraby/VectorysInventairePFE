import { Injectable } from '@angular/core';

export interface UserPreferences {
  notificationsEcarts: boolean;
  notificationsFinEmplacement: boolean;
  notificationsSon: boolean;
  inventaireDefautId: number | null;
  langue: 'fr';
}

const KEY = 'vectorys_settings_prefs';

const DEFAULTS: UserPreferences = {
  notificationsEcarts: true,
  notificationsFinEmplacement: true,
  notificationsSon: false,
  inventaireDefautId: null,
  langue: 'fr'
};

@Injectable({ providedIn: 'root' })
export class SettingsPreferencesService {
  load(): UserPreferences {
    const raw = localStorage.getItem(KEY);
    if (!raw) {
      return { ...DEFAULTS };
    }
    try {
      return { ...DEFAULTS, ...(JSON.parse(raw) as Partial<UserPreferences>) };
    } catch {
      return { ...DEFAULTS };
    }
  }

  save(prefs: UserPreferences): void {
    localStorage.setItem(KEY, JSON.stringify(prefs));
  }

  /** Prefer saved default inventaire when it still exists in the list. */
  resolveInventaireId(ids: number[]): number | null {
    if (!ids.length) return null;
    const preferred = this.load().inventaireDefautId;
    if (preferred != null && ids.includes(preferred)) {
      return preferred;
    }
    return ids[0];
  }
}
