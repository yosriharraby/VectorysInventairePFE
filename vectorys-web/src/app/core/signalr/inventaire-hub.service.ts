import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';
import { FinComptageEvent, NouveauScanEvent } from '../../shared/models/monitoring.model';

@Injectable({ providedIn: 'root' })
export class InventaireHubService {
  private readonly auth = inject(AuthService);
  private connection: signalR.HubConnection | null = null;
  private joinedInventaireId: number | null = null;

  readonly nouveauScan$ = new Subject<NouveauScanEvent>();
  readonly finComptage$ = new Subject<FinComptageEvent>();
  readonly connectionState$ = new Subject<'connected' | 'disconnected' | 'connecting'>();

  async connect(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    const token = this.auth.getToken();
    if (!token) {
      throw new Error('Token manquant pour SignalR.');
    }

    this.connectionState$.next('connecting');

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.signalRUrl}/inventaire?access_token=${encodeURIComponent(token)}`)
      .withAutomaticReconnect()
      .build();

    this.connection.on('NouveauScan', (payload: NouveauScanEvent) => {
      this.nouveauScan$.next(payload);
    });

    this.connection.on('FinComptage', (payload: FinComptageEvent) => {
      this.finComptage$.next(payload);
    });

    this.connection.onreconnected(async () => {
      this.connectionState$.next('connected');
      if (this.joinedInventaireId != null) {
        await this.joinInventaireGroup(this.joinedInventaireId);
      }
    });

    this.connection.onclose(() => this.connectionState$.next('disconnected'));

    await this.connection.start();
    this.connectionState$.next('connected');
  }

  async joinInventaireGroup(inventaireId: number): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      await this.connect();
    }
    if (this.joinedInventaireId != null && this.joinedInventaireId !== inventaireId) {
      await this.connection!.invoke('LeaveInventaireGroup', this.joinedInventaireId);
    }
    await this.connection!.invoke('JoinInventaireGroup', inventaireId);
    this.joinedInventaireId = inventaireId;
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    await this.connection.stop();
    this.connection = null;
    this.joinedInventaireId = null;
    this.connectionState$.next('disconnected');
  }
}
