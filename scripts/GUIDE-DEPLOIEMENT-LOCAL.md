# Déploiement local — Vectorys Inventaire (Sprint 9)

Objectif : publier API + portail Angular + APK Flutter sur un serveur Windows du réseau LAN.

## Prérequis
- Windows Server / PC serveur
- .NET 9 Runtime (ASP.NET Core Hosting Bundle si IIS)
- SQL Server Express (ou instance existante) + base VectorysDB
- Node.js 20+ (build Angular)
- Flutter SDK (build APK)
- Pare-feu : ouvrir le port API (5000) et éventuellement 80/443

## 1. Base de données
1. Copier / adapter la chaîne de connexion dans `appsettings.Production.json`
   (partir de `appsettings.Production.example.json`).
2. Depuis le poste de build :
   ```powershell
   cd src\VectorysInventaire.API
   $env:ASPNETCORE_ENVIRONMENT = "Production"
   # Option : appliquer migrations via Development d'abord
   dotnet ef database update --project ..\VectorysInventaire.Infrastructure
   ```
3. Vérifier le seed des utilisateurs (ou créer les comptes en prod).

## 2. Publier l'API
```powershell
cd src\VectorysInventaire.API
dotnet publish -c Release -o C:\Vectorys\api
# Copier appsettings.Production.json dans C:\Vectorys\api
```

Démarrage Kestrel (écoute toutes interfaces) :
```powershell
cd C:\Vectorys\api
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "http://0.0.0.0:5000"
.\VectorysInventaire.API.exe
```

Ou installer comme service Windows / site IIS pointant vers le dossier publish.

Vérifier : `http://SERVER_IP:5000/swagger` (Swagger désactivé hors Development —
utiliser un health check ou login Postman en Production).

## 3. Portail Angular
```powershell
cd vectorys-web
# Éditer environment.production.ts : remplacer REPLACE_WITH_SERVER_IP
npm ci
npm run build -- --configuration=production
# Servir dist/vectorys-web/browser via IIS ou npx serve
```

CORS : dans `appsettings.Production.json`, ajouter l’origine exacte du portail
(ex. `http://192.168.1.10`).

## 4. Application mobile
```powershell
cd vectorys_mobile
flutter build apk --release --dart-define=API_BASE_URL=http://SERVER_IP:5000/api
```
Installer l’APK sur les terminaux (même Wi‑Fi / LAN que le serveur).

## 5. Checklist de validation LAN
- [ ] Login superviseur depuis un autre PC
- [ ] Création inventaire + import Excel
- [ ] Login agent mobile + scan
- [ ] SignalR monitoring
- [ ] Branche A / B, clôture, export Excel
- [ ] Offline sync mobile

## Comptes de démo (seed Development)
À remplacer en production :
  - Superviseurs web :
    - marwen.bensalah@vectorys.local / Super123!
    - siwar.zrelli@vectorys.local / Super123!
    - karim.ferchichi@vectorys.local / Super123!
    - (alias) superviseur@vectorys.local / Super123!
  - Agents mobile (code personnel = mot de passe) :
    - AG001 Sami Trabelsi
    - AG002 Nour Mansouri
    - AG003 Hedi Jebali
    - AG004 Amira Gharbi
    - AG005 Yassine Bouazizi


## Secrets
Ne jamais committer `appsettings.Production.json` avec le vrai secret JWT.
Utiliser variables d’environnement ou User Secrets hors dépôt.
