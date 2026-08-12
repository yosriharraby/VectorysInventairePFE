import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { AuthService } from '../../../core/auth/auth.service';
import { InventaireApiService } from '../../../core/services/inventaire-api.service';
import { Agent, agentDisplayName } from '../../../shared/models/agent.model';
import { CreateInventaireResult } from '../../../shared/models/inventaire.model';

@Component({
  selector: 'app-create-inventaire',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatRadioModule,
    MatButtonModule,
    MatMenuModule
  ],
  templateUrl: './create-inventaire.component.html',
  styleUrl: './create-inventaire.component.scss'
})
export class CreateInventaireComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(InventaireApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly user = this.auth.getCurrentUser();
  readonly agentDisplayName = agentDisplayName;

  readonly infoForm = this.fb.nonNullable.group({
    intitule: ['', [Validators.required, Validators.maxLength(200)]],
    zone: ['Vectorys Tunis - Radès', [Validators.required, Validators.maxLength(100)]],
    dateDebut: [new Date().toISOString().slice(0, 10), Validators.required],
    dateFin: [''],
    typePassage: ['C1' as 'C1' | 'C2', Validators.required]
  });

  availableAgents: Agent[] = [];
  selectedAgents: Agent[] = [];
  agentsError: string | null = null;

  selectedFile: File | null = null;
  fileError: string | null = null;
  agentsSelectionError: string | null = null;
  submitting = false;
  error: string | null = null;
  result: CreateInventaireResult | null = null;

  get fileName(): string | null {
    return this.selectedFile?.name ?? null;
  }

  get unselectedAgents(): Agent[] {
    const selectedIds = new Set(this.selectedAgents.map((a) => a.id));
    return this.availableAgents.filter((a) => !selectedIds.has(a.id));
  }

  ngOnInit(): void {
    this.api.getAgents().subscribe({
      next: (agents) => {
        this.availableAgents = agents;
        this.agentsError = null;
      },
      error: (err) => {
        const status = err?.status as number | undefined;
        this.agentsError =
          status === 404
            ? 'Endpoint /api/agents introuvable — redémarrez l’API (dotnet run) puis rechargez la page.'
            : 'Impossible de charger la liste des agents. Vérifiez que l’API tourne sur http://localhost:5000.';
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
    this.fileError = null;
  }

  onFileDropped(file: File | null): void {
    this.selectedFile = file;
    this.fileError = null;
  }

  addAgent(agent: Agent): void {
    if (this.selectedAgents.some((a) => a.id === agent.id)) {
      return;
    }
    this.selectedAgents = [...this.selectedAgents, agent];
    this.agentsSelectionError = null;
  }

  removeAgent(agent: Agent): void {
    this.selectedAgents = this.selectedAgents.filter((a) => a.id !== agent.id);
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }

  cancel(): void {
    void this.router.navigate(['/dashboard']);
  }

  private importAndFinish(created: CreateInventaireResult, passageId: number, codeAgents: string[]): void {
    this.api.importStock(passageId, this.selectedFile!, codeAgents).subscribe({
      next: () => {
        this.result = created;
        this.submitting = false;
        void this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.error = "Inventaire créé mais échec de l'import Excel.";
        this.submitting = false;
      }
    });
  }

  submitWizard(): void {
    this.error = null;
    this.agentsSelectionError = null;

    if (this.infoForm.invalid) {
      this.infoForm.markAllAsTouched();
      return;
    }
    if (this.selectedAgents.length === 0) {
      this.agentsSelectionError = 'Sélectionnez au moins un agent affecté.';
      return;
    }
    if (!this.selectedFile) {
      this.fileError = 'Sélectionnez un fichier Excel (.xlsx).';
      return;
    }

    const raw = this.infoForm.getRawValue();
    const codeAgents = this.selectedAgents.map((a) => a.codePersonnel);
    const typePassage = raw.typePassage;
    this.submitting = true;

    this.api
      .createInventaire({
        intitule: raw.intitule,
        zone: raw.zone,
        dateDebut: raw.dateDebut,
        dateFin: raw.dateFin || null
      })
      .subscribe({
        next: (created) => {
          if (typePassage === 'C2') {
            // C1 est créé automatiquement ; on crée ensuite le passage C2 et on y importe le stock.
            this.api.createPassage(created.inventaire.id, 'C2').subscribe({
              next: (passageC2) => {
                this.importAndFinish(created, passageC2.id, codeAgents);
              },
              error: () => {
                this.error =
                  "Inventaire créé (C1) mais échec de la création du passage C2. Vérifiez qu'un C2 n'existe pas déjà.";
                this.submitting = false;
              }
            });
          } else {
            this.importAndFinish(created, created.passageC1Id, codeAgents);
          }
        },
        error: () => {
          this.error = "Échec de la création de l'inventaire.";
          this.submitting = false;
        }
      });
  }
}
