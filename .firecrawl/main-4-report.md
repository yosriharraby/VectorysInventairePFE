# Rapport de Projet de Fin d’Études

Présenté en vue de l’obtention du

Diplôme National d’Ingénieur en Sciences Appliquées et Technologiques
Spécialité : Génie Logiciel

Réalisé par

Harrabi Yosri

# Conception et développement d’une plateforme d’inventaire logistique multi-canal

Encadrant professionnel : Monsieur Ferchichi Marwen

Encadrant académique : Madame Zrelli Siwar

Chef de projet SI

Ingénieur Informatique

---

J’autorise l’étudiant à faire le dépôt de son rapport de stage en vue d’une soutenance.

Encadrant professionnel,M. Ferchichi Marwen

Signature et cachet

J’autorise l’étudiant à faire le dépôt de son rapport de stage en vue d’une soutenance.

Encadrant académique,M. Zrelli Siwar

Signature

---

## Dédicace

Je dédie ce modeste travail,

Àma très chère mère, pour son amour inconditionnel, sa confiance, ses sacrifices et son
soutien constant, qui ont été la source de ma motivation et de ma réussite. Ce travail est le fruit de ses
encouragements et de ses prières, et je lui adresse toute ma profonde gratitude et ma reconnaissance.

Àma famille, pour son affection, sa présence et son soutien précieux tout au long de mon
parcours.

Àmes amis et collègues, pour leur amitié, leur entraide et les moments de partage qui ont
enrichi cette aventure.

Àtoutes les personnesqui ont cru en moi et qui m’ont accompagné de près ou de loin, je
dédie ce travail avec une profonde gratitude.

*Harrabi YOSRI*

---

## Remerciements

Au terme de ce projet de fin d’études, je tiens à exprimer ma profonde gratitude à toutes les
personnes qui ont contribué, de près ou de loin, à la réalisation de ce travail et qui m’ont accompagné
durant cette expérience enrichissante.

Je remercie particulièrement mon encadrant professionnel au sein deVECTORYS Tunisie,
Mr. Ferchichi Marwen, pour son accompagnement, sa disponibilité, ses conseils pertinents et la
confiance qu’il m’a accordée tout au long de mon stage. Son encadrement et son expertise ont largement
contribué à la réussite de ce projet.

J’adresse également mes sincères remerciements à mon encadrante académique,Madame Zrelli
Siwar, ingénieure informatique à l’École Supérieure Privée d’Ingénierie et de Technologie, pour son
suivi attentif, ses orientations précieuses et son soutien méthodologique durant l’élaboration de ce
travail.

Mes remerciements s’adressent de même aux membres du jury pour l’intérêt porté à ce travail
et pour le temps consacré à son évaluation.

Enfin, j’exprime toute ma reconnaissance à ma famille et à mes amis pour leur soutien, leurs
encouragements et leur présence constante, qui ont été une source de motivation essentielle jusqu’à
l’aboutissement de ce projet.

---

## Table des matières

## Introduction générale

1.1.1 Présentation de l’organisme d’accueil . . . . . . . . . . . . . . . . . . . . . . . .5
1.1.2 Contexte et objectifs du projet . . . . . . . . . . . . . . . . . . . . . . . . . . .6

1.2.1 Étude du processus interne d’inventaire VECTORYS . . . . . . . . . . . . . . .6
1.2.2 Étude des outils Excel et ERP existants . . . . . . . . . . . . . . . . . . . . . .7
1.2.3 Étude des solutions WMS et applications de scan . . . . . . . . . . . . . . . . .8

1.5 Méthodologie de gestion de projet . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .10

Introduction générale 1

# 1 Cadre général du projet 3
Introduction . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

2 Spécification et conception globale

2.1 Spécification des besoins . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .15
2.1.1 Identification des acteurs . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .15
2.1.2 Spécification des besoins fonctionnels . . . . . . . . . . . . . . . . . . . . . . . .16
2.1.3 Spécification des besoins non fonctionnels . . . . . . . . . . . . . . . . . . . . .18
2.2 Diagramme de cas d’utilisation global . . . . . . . . . . . . . . . . . . . . . . . . . . .19
2.3 Diagramme de classes global . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .21
2.4 Pilotage du projet avec Scrum . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .23
2.4.1 Équipe et rôles Scrum . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .23

2.4.3 Répartition des releases et sprints . . . . . . . . . . . . . . . . . . . . . . . . . .25

---

2.5.1 Architecture logique de l'application . 27
2.5.2 Architecture physique de l'application . 28

2.6 Environnement de travail . 29
2.6.1 Langage de modélisation . 29
2.6.2 Environnement matériel . 29
2.6.3 Environnement logiciel . 30
Conclusion . 31

# 3 Release 1 : sécurité, préparation et comptage connecté 32
Introduction . 34

3.1 Phase préparatoire : initialisation du socle technique . 34
    3.1.1 Mise en place de l'environnement de développement . 34
    3.1.2 Creation et organisation des projets . 34
    3.1.3 Initialisation de la persistance . 34
    3.1.4 Préparation des services transversaux . 35
    3.1.5 Synthèse des éléments préparés . 35

3.2 Sprint 1 : authentification et autorisation . 35
    3.2.1 Objectifs du Sprint 1 . 35
    3.2.2 Backlog du Sprint 1 . 36
    3.2.3 Planification des tâches du Sprint 1 . 36
    3.2.4 Conception du Sprint 1 . 36
    3.2.5 Réalisation du Sprint 1 . 36
    3.2.6 Tests du Sprint 1 . 37
    3.2.7 Revue et rétrospective du Sprint 1 . 37

3.3 Sprint 2 : inventaire, import Excel et assignation . 38
    3.3.1 Objectifs du Sprint 2 . 38
    3.3.2 Backlog du Sprint 2 . 38
    3.3.3 Planification des tâches du Sprint 2 . 38
    3.3.4 Conception du Sprint 2 . 38
    3.3.5 Réalisation du Sprint 2 . 39
    3.3.6 Tests du Sprint 2 . 41
    3.3.7 Revue et rétrospective du Sprint 2 . 41

3.4 Sprint 3 : scan, quotas et temps rêel . 41
    3.4.1 Objectifs du Sprint 3 . 41

---

3.4.3 Planification des tâches du Sprint 3 . . . . . . . . . . . . . . . . . . . . . . . . .41

3.4.7 Revue et rétrospective du Sprint 3 . . . . . . . . . . . . . . . . . . . . . . . . .42

4 Release 2 : résilience et traitement des écarts

4.1 Sprint 4 : mode hors ligne et synchronisation batch . . . . . . . . . . . . . . . . . . . .45

4.1.3 Planification des tâches du Sprint 4 . . . . . . . . . . . . . . . . . . . . . . . . .45

3.4.2 Backlog du Sprint 3 . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

4.1.7 Revue et rétrospective du Sprint 4 . . . . . . . . . . . . . . . . . . . . . . . . .46
4.2 Sprint 5 : détection et traitement des écarts . . . . . . . . . . . . . . . . . . . . . . . .46

4.2.3 Planification des tâches du Sprint 5 . . . . . . . . . . . . . . . . . . . . . . . . .46

4.2.7 Revue et rétrospective du Sprint 5 . . . . . . . . . . . . . . . . . . . . . . . . .47
4.3 Sprint 6 : clôture, consolidation et export . . . . . . . . . . . . . . . . . . . . . . . . .47

4.3.3 Planification des tâches du Sprint 6 . . . . . . . . . . . . . . . . . . . . . . . . .48

4.3.7 Revue et rétrospective du Sprint 6 . . . . . . . . . . . . . . . . . . . . . . . . .48

---

5 Release 3 : pilotage, validation et mise en service

5.1 Sprint 7 : dashboard KPI et analytics . . . . . . . . . . . . . . . . . . . . . . . . . . . .51

5.1.3 Planification des tâches du Sprint 7 . . . . . . . . . . . . . . . . . . . . . . . . .51

5.1.7 Revue et rétrospective du Sprint 7 . . . . . . . . . . . . . . . . . . . . . . . . .52
5.2 Sprint 8 : tests, documentation et validation . . . . . . . . . . . . . . . . . . . . . . . .52

5.2.3 Planification des tâches du Sprint 8 . . . . . . . . . . . . . . . . . . . . . . . . .52

Conclusion . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

5.2.7 Revue et rétrospective du Sprint 8 . . . . . . . . . . . . . . . . . . . . . . . . .53
5.3 Sprint 9 : déploiement local . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .53

5.3.3 Planification des tâches du Sprint 9 . . . . . . . . . . . . . . . . . . . . . . . . .53

5.3.7 Revue et rétrospective du Sprint 9 . . . . . . . . . . . . . . . . . . . . . . . . .54

---

## Table des figures

1.1 Positionnement du projet dans le contexte VECTORYS. . . . . . . . . . . . . . . . .5
1.2 Processus d’inventaire existant avant digitalisation. . . . . . . . . . . . . . . . . . . .7
1.3 Positionnement de Vectorys Inventaire face à l’existant . . . . . . . . . . . . . . . . . .8
1.4 Architecture three-tier de la plateforme Vectorys Inventaire. . . . . . . . . . . . . . .9
1.5 Flux métier principal du cycle d’inventaire Vectorys . . . . . . . . . . . . . . . . . . . .10
1.6 Organisation de la phase préparatoire et des sprints 1 à 9. . . . . . . . . . . . . . . .11
1.7 Cadre Scrum et répartition des rôles dans le projet . . . . . . . . . . . . . . . . . . . .11
2.1 Diagramme de cas d’utilisation global de Vectorys Inventaire. . . . . . . . . . . . . .20
2.2 Diagramme de classes global du domaine métier . . . . . . . . . . . . . . . . . . . . . .22
2.3 Architecture physique de déploiement sur le réseau local . . . . . . . . . . . . . . . . .28
3.1 Interface d’authentification du superviseur . . . . . . . . . . . . . . . . . . . . . . . . .37
3.2 Interface de création et de préparation d’un inventaire. . . . . . . . . . . . . . . . . .40

viii

---

## Liste des tableaux

1.1 Analyse des outils existants chez VECTORYS . 7
1.3 Stack technologique de la solution . 10
1.5 Cérémonies Scrum appliquées au projet . 11

2.1 Acteurs de la plateforme Vectorys Inventaire . 15
2.3 Besoins fonctionnels de la plateforme . 16
2.5 Besoins non fonctionnels de la plateforme . 18
2.7 Répartition des responsabilités du projet Scrum . 23
2.9 Product Backlog global ordonné . 24
2.11 Répartition des releases et des sprints . 26
2.13 Couches logiques et responsabilités . 27
2.15 Environnement matériel du projet . 29
2.17 Environnement logiciel du projet . 30

3.1 Synthèse de l'initialisation du socle technique . 35
3.3 Planification des tâches du Sprint 1 . 36
3.5 Planification des tâches du Sprint 2 . 38
3.7 Planification des tâches du Sprint 3 . 41

4.1 Planification des tâches du Sprint 4 . 45
4.3 Planification des tâches du Sprint 5 . 46
4.5 Planification des tâches du Sprint 6 . 48

5.1 Planification des tâches du Sprint 7 . 51
5.3 Planification des tâches du Sprint 8 . 52
5.5 Planification des tâches du Sprint 9 . 53

ix

---

## Liste des abréviations

| Sigle | Signification |
| --- | --- |
| API | Application Programming Interface |
| C1/C2 | Premier/second passage de comptage d'inventaire |
| CORS | Cross-Origin Resource Sharing |
| CQRS | Command Query Responsibility Segregation |
| DTO | Data Transfer Object |
| E2E | End-to-End(bout en bout) |
| EF Core | Entity Framework Core |
| ERP | Enterprise Resource Planning |
| HTTP/HTTPS | HyperText Transfer Protocol(Secure) |
| IIS | Internet Information Services |
| JWT | JSON Web Token |
| KPI | Key Performance Indicator(indicateur de performance) |
| LAN | Local Area Network |
| ORM | Object-Relational Mapping |
| PDA | Personal Digital Assistant(terminal mobile) |
| PFE | Projet de Fin d&#x27;Études |
| REST | Representational State Transfer |
| RF/RNF | Exigence Fonctionnelle/Non Fonctionnelle |
| SGBD | Système de Gestion de Base de Données |
| SQL | Structured Query Language |
| SSMS | SQL Server Management Studio |
| TTL | Time To Live(durée de validité du jeton) |
| UC | Use Case(cas d&#x27;utilisation) |
| UML | Unified Modeling Language |
| US | Unité de Stock(code scanné) |
| WMS | Warehouse Management System |

x

---

## Introduction générale

La gestion des stocks constitue un enjeu stratégique pour les opérateurs logistiques. Une discordance
entre le stock théorique enregistré dans les systèmes d’information et la réalité physique observée sur le
terrain engendre des pertes financières, des ruptures de service et une dégradation de la relation client.
Dans ce contexte, l’inventaire physique — opération consistant à compter et contrôler l’ensemble
des articles présents dans un entrepôt — demeure une pratique incontournable, malgré sa lourdeur
organisationnelle.

VECTORYS Tunisie, société spécialisée dans la logistique et la distribution, fait face à
cette réalité au quotidien. Les inventaires y sont encore largement pilotés à l’aide d’outils hétérogènes,
tels que les fichiers Excel, les terminaux de saisie et les logiciels ERP hérités, qui ne facilitent ni la
coordination des équipes terrain, ni le traitement structuré des écarts constatés entre le stock attendu
et le stock effectivement compté. Les agents doivent souvent alterner entre plusieurs supports, tandis
que les superviseurs manquent de visibilité en temps réel sur l’avancement des opérations.

C’est dans ce cadre que s’inscrit le présent projet de fin d’études, intitulé « Conception et
développement d’une plateforme d’inventaire logistique multi-canal ». L’objectif est de concevoir et
de réaliser une solution logicielle intégrée permettant de couvrir l’ensemble du cycle d’inventaire :
préparation de la mission, comptage terrain par scan de codes-barres, synchronisation des données,
détection automatique des écarts, pilotage par indicateurs de performance et aide à la décision pour le
superviseur.

La plateforme développée, nommée*Vectorys Inventaire*, repose sur une architecture multi-canal
articulée autour de trois composants complémentaires :

–une API REST backend développée en ASP.NET Core 9, structurée selon les principes de la
Clean Architecture et du pattern CQRS (MediatR);

–une application web de supervision, réalisée avec Angular 19, destinée aux superviseurs d’inventaire;

–une application mobile terrain, développée en Flutter, déployée sur terminaux Android équipés
de lecteurs de codes-barres, à l’usage des agents de comptage.

Les données métier sont centralisées dans une base Microsoft SQL Server, garantissant la
cohérence transactionnelle des opérations. La communication temps réel entre le terrain et la salle
de contrôle est assurée par SignalR, tandis qu’un mécanisme de mode hors ligne, basé sur SQLite local
et une synchronisation différée, permet aux agents de poursuivre le comptage en cas d’instabilité du
réseau Wi-Fi en entrepôt.

La réalisation du projet a été conduite selon une approche agile Scrum. Une phase préparatoire

1 a d’abord permis d’initialiser le socle technique, puis neuf sprints numérotés de 1 à 9 ont été répartis
équitablement en trois releases, depuis la sécurisation des accès jusqu’aux modules de détection d’écarts,
de tableaux de bord KPI et de préparation au déploiement.
Le présent rapport décrit de manière progressive le travail accompli. Le chapitre 1 présente le
cadre du projet, tandis que le chapitre 2 formalise les besoins, les diagrammes globaux, l’architecture et
la planification Scrum. Les chapitres 3, 4 et 5 détaillent ensuite les trois releases : sécurité, préparation
et comptage connecté; résilience et traitement des écarts; enfin pilotage, validation et mise en service.

2

---

# Cadre général du projet

---

## Plan

Introduction . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

Méthodologie de gestion de projet . . . . . . . . . . . . . . . . . . . . . . . .10

---

## Introduction

Tout projet informatique industriel doit être compris à la lumière du contexte dans lequel il
s’inscrit. Avant d’aborder les aspects techniques, il convient de présenter l’entreprise d’accueil, les
contraintes opérationnelles du domaine logistique et les attentes exprimées par les utilisateurs finaux.
Ce chapitre pose ainsi les bases du projet*Vectorys Inventaire*: cadre général, analyse de l’existant,
problématique, solution proposée et méthodologie de gestion adoptée.

## 1.1Cadre du projet

## 1.1.1Présentation de l’organisme d’accueil

VECTORYS Tunisieest une entreprise opérant dans le secteur de la logistique, du transport
international et de la supply chain. Son activité s’inscrit dans un environnement où la fiabilité des
stocks, la rapidité de traitement et la traçabilité des flux constituent des leviers essentiels de performance.
L’entreprise intervient notamment dans la gestion d’entrepôts, la préparation de commandes, le suivi
des mouvements de marchandises et le contrôle des opérations logistiques.

Dans ce contexte, l’inventaire physique représente une opération critique. Il permet de comparer
le stock théorique issu du système d’information avec le stock réellement constaté dans l’entrepôt.
Cette opération mobilise plusieurs profils : des superviseurs, chargés de préparer l’inventaire, suivre
l’avancement et traiter les écarts; des agents terrain, responsables du scan des unités de stock; et un
système d’information central, garant de la cohérence des données.

Le stage s’est déroulé au sein deVECTORYS Tunisiesous l’encadrement professionnel
deMr. Ferchichi Marwen, chef de projet SI, qui a apporté une vision métier précise sur les
flux d’inventaire réels et les contraintes opérationnelles de l’entrepôt. L’encadrement académique a
été assuré parMadame Zrelli Siwar, ingénieure informatique à TEK-UP, garantissant la rigueur
méthodologique et la conformité du rapport aux exigences du diplôme d’ingénieur.

Figure 1.1:Positionnement du projet dans le contexte VECTORYS

5

---

## 1.1.2Contexte et objectifs du projet

Le projet répond à un besoin concret identifié sur le terrain : disposer d’une plateforme unifiée
capable d’orchestrer un inventaire de bout en bout, depuis l’import du stock théorique jusqu’à l’exploitation
des résultats par des indicateurs de performance.

Les objectifs principaux du projet sont les suivants :

–Digitaliser la préparation d’inventaire: permettre au superviseur de créer une mission,
d’importer un fichier Excel structuré et de générer automatiquement un premier passage de
comptage.

–Outiller le comptage terrain: fournir aux agents une application mobile ergonomique,
optimisée pour le scan en boucle, avec gestion des quotas par code US et clôture automatique
de l’emplacement.

–Assurer la traçabilité et la résilience: enregistrer chaque scan avec horodatage et identifiant
agent, tout en supportant le mode hors ligne et la resynchronisation sans perte ni doublon.

–Automatiser la détection des écarts: comparer systématiquement le stock théorique et les
quantités scannées, catégoriser les anomalies et proposer des workflows de correction.

–Offrir un pilotage en temps réel: mettre à disposition du superviseur un tableau de bord
web actualisé via SignalR, complété par un module d’analytics KPI.

Ces objectifs traduisent une ambition claire : passer d’un inventaire documenté de façon dispersée
à un processus traçable, collaboratif et mesurable, accessible simultanément depuis le poste superviseur
et depuis le terrain.

## 1.2Étude de l’existant

L’étude de l’existant vise à comprendre les pratiques actuelles et à identifier les limites qui
justifient la mise en place d’une solution spécifique à VECTORYS. Elle ne se limite pas à une
comparaison technique : elle analyse le processus réel d’inventaire, les outils mobilisés et les solutions
génériques disponibles sur le marché.

## 1.2.1Étude du processus interne d’inventaire VECTORYS

Avant la mise en place de la plateforme, le déroulement type d’un inventaire reposait sur une
chaîne d’actions largement manuelle : extraction du stock théorique depuis le système d’information,
préparation d’un fichier Excel, répartition des zones entre agents, comptage terrain, consolidation des
résultats et analyse des écarts.

6

---

Cette organisation présente plusieurs limites :

–absence d’une source de vérité unique consultable en temps réel;

–difficulté à suivre l’avancement par agent, zone ou emplacement;

–risque de doublons ou d’oublis lors de la consolidation manuelle;

–absence de mécanisme standardisé pour traiter les écarts;

–dépendance forte à la qualité des fichiers Excel et aux échanges informels.

Figure 1.2:Processus d’inventaire existant avant digitalisation

## 1.2.2Étude des outils Excel et ERP existants

Les fichiers Excel occupent une place centrale dans l’organisation actuelle. Ils permettent de
préparer le stock théorique, de filtrer les emplacements, de répartir les zones et de consolider les résultats
après comptage. Cette souplesse constitue un avantage à court terme, mais elle devient une limite dès
que le volume de données augmente ou que plusieurs agents travaillent simultanément.

L’ERP fournit la base théorique de départ, mais il ne couvre pas à lui seul les besoins terrain
du comptage mobile, de l’assignation stricte agent-emplacement, de la synchronisation hors ligne et du
suivi temps réel. L’écart entre les données théoriques et les opérations physiques nécessite donc une
couche applicative dédiée.

Tableau 1.1:Analyse des outils existants chez VECTORYS

| Outil existant | Apport | Limites observées |
| --- | --- | --- |
| ERP / système stock | Source du stock théorique et référentiel de départ | Peu adapté au scan terrain, au suivi temps réel et aux workflows de recomptage |

7

---

Chapitre 1. Cadre général du projet

| Outil existant | Apport | Limites observées |
| --- | --- | --- |
| Excel | Souplesse de préparation, filtrage et partage rapide | Risque d&#x27;erreurs, versions multiples, consolidation manuelle, traçabilité limitée |
| Terminaux ou scans isolés | Collecte ponctuelle de codes-barres | Pas de logique C1/C2, pas de quotas serveur, synchronisation limitée |

## 1.2.3Étude des solutions WMS et applications de scan

Les solutions WMS du marché proposent des modules d’inventaire avancés. Cependant, elles sont
souvent coûteuses, lourdes à paramétrer et peu flexibles lorsqu’il s’agit d’épouser un processus spécifique
à une entreprise donnée. Dans le cas de VECTORYS, les besoins portent sur un périmètre ciblé : import
Excel structuré, assignation par CodeAgent, comptage mobile, mode hors ligne, détection d’écarts et
pilotage KPI.

Les applications de scan génériques permettent de lire des codes-barres, mais elles ne couvrent pas la
logique métier essentielle : passages C1/C2, quotas par code US, clôture automatique de l’emplacement,
synchronisation batch et tableau de bord superviseur.

Figure 1.3:Positionnement de Vectorys Inventaire face à l’existant

En synthèse, l’étude de l’existant met en évidence un espace fonctionnel non couvert : une solution
légère, dédiée au cycle d’inventaire de VECTORYS, intégrant web, mobile et API, tout en restant
alignée sur les règles métier du dépôt.

## 1.3Problématique

La problématique centrale de ce projet peut être formulée ainsi :

Comment concevoir et développer une plateforme d’inventaire logistique multi-canal, fiable
et évolutive, capable de synchroniser en temps réel les opérations de comptage terrain avec

8 le pilotage superviseur, tout en garantissant la résilience hors connexion et le traitement
structuré des écarts entre stock théorique et stock constaté?

Cette question soulève plusieurs défis techniques et organisationnels :

–Interopérabilité multi-canal: les clients API, web et mobile doivent partager un modèle de
données cohérent et des règles métier identiques.

–Performance terrain: la boucle de scan doit rester fluide, avec une latence minimale et un
feedback immédiat.

–Résilience réseau: les zones à couverture Wi-Fi variable ne doivent pas entraîner de perte
des données saisies hors ligne.

–Intégrité des données: l’assignation des emplacements aux agents, les quotas de scan et la
détection d’écarts doivent être appliqués de façon stricte côté serveur.

–Pilotage décisionnel: le superviseur doit disposer d’indicateurs agrégés pour arbitrer
rapidement.

## 1.4Solution proposée

La solution retenue est la plateforme*Vectorys Inventaire*, organisée autour d’une architecture three-tier
et de flux métier formalisés.

Figure 1.4:Architecture three-tier de la plateforme Vectorys Inventaire
[Image: Im6]

Le système articule trois couches complémentaires :

–Couche présentation: application web Angular pour la supervision et application mobile
Flutter pour le comptage terrain.

9

---

–Couche métier / API: API ASP.NET Core sécurisée par JWT, organisée avec MediatR,
FluentValidation et SignalR.

–Couche données: base SQL Server modélisant les inventaires, passages, emplacements, stocks
théoriques, scans, écarts et utilisateurs.

Figure 1.5:Flux métier principal du cycle d’inventaire Vectorys
[Image: Im7]

Les flux structurants sont : préparation, comptage terrain, détection d’écarts, résolution puis clôture
et pilotage. Cette organisation permet de couvrir l’ensemble du cycle métier sans dupliquer les règles
dans les clients web et mobile.

Tableau 1.3:Stack technologique de la solution

| Composant | Technologie | Rôle |
| --- | --- | --- |
| Backend API | ASP.NET Core 9, C# | Logique métier, REST, SignalR et JWT |
| ORM | Entity Framework Core 9 | Accès SQL Server et migrations |
| Application web | Angular 19, TypeScript, Chart.js | Interface superviseur et KPI |
| Application mobile | Flutter, Dart, SQLite | Scan terrain et mode hors ligne |
| Base de données | Microsoft SQL Server | Persistance centralisée |
| Import / export | ClosedXML | Lecture et génération de fichiers Excel |
| Temps réel | SignalR | Notifications de scan et de clôture |

## 1.5Méthodologie de gestion de projet

## 1.5.1Méthodologie Scrum

Le projet a été mené selon le cadre Scrum, méthode agile itérative particulièrement adaptée aux projets
logiciels dont les besoins évoluent au fil des retours utilisateurs. Le découpage en sprints de une à deux
semaines a permis de livrer régulièrement des incréments testables, depuis les fondations techniques
jusqu’aux modules avancés comme les KPI, les tests et la préparation au déploiement.

10

---

Figure 1.6:Organisation de la phase préparatoire et des sprints 1 à 9

## 1.5.2Rôles Scrum

Les rôles Scrum ont été répartis comme suit :

–Product Owner: représentant métier, priorisant le backlog et validant les livrables.

–Scrum Master / développeur: stagiaire, responsable de l’organisation des sprints, de
l’implémentation et de la documentation.

–Équipe de développement: le stagiaire, appuyé ponctuellement par l’encadrant pour les
arbitrages techniques et métier.

Figure 1.7:Cadre Scrum et répartition des rôles dans le projet
[Image: Im8]

## 1.5.3Cérémonies Scrum

Les cérémonies appliquées incluent la planification de sprint, le daily stand-up, la revue de sprint
et la rétrospective. Elles ont permis de maintenir un rythme régulier, de valider les incréments avec
l’encadrant professionnel et d’ajuster les priorités selon les contraintes métier.

Tableau 1.5:Cérémonies Scrum appliquées au projet

11

---

Chapitre 1. Cadre général du projet

| Cérémonie | Rôle dans le projet |
| --- | --- |
| Sprint planning | Définir les objectifs, sélectionner les user stories et planifier l&#x27;incrément |
| Daily stand-up | Suivre l&#x27;avancement, identifier les blocages et coordonner les tâches |
| Sprint review | Présenter les fonctionnalités réalisées et recueillir les retours métier |
| Rétrospective | Identifier les améliorations de méthode pour le sprint suivant |

Après une phase préparatoire dédiée au socle technique, le découpage en neuf sprints répartis dans trois
releases garantit une montée en charge progressive de la complexité : la sécurité et le premier parcours
connecté précèdent les mécanismes de résilience, de consolidation, d’analyse et de mise en production.

## Conclusion

Ce premier chapitre a permis de situer le projet de fin d’études dans son contexte organisationnel
et opérationnel. VECTORYS Tunisie exprime un besoin légitime de modernisation des inventaires
physiques, face à des pratiques encore trop manuelles et à des outils existants insuffisamment intégrés.
La problématique retenue met en avant les enjeux de synchronisation multi-canal, de résilience et de
traitement structuré des écarts.

La solution*Vectorys Inventaire*y répond par une architecture three-tier articulant une API .NET, une
interface web Angular et une application mobile Flutter. Son pilotage repose sur une phase préparatoire
hors sprint, suivie de neuf sprints regroupés en trois releases équilibrées. Le chapitre suivant détaillera
l’analyse des besoins fonctionnels et non fonctionnels, les acteurs du système et les diagrammes de
spécification qui fondent la conception technique.

12

---

# Spécification et conception globale

---

## Plan

Spécification des besoins . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .15
Diagramme de cas d’utilisation global . . . . . . . . . . . . . . . . . . . . . .19
Diagramme de classes global . . . . . . . . . . . . . . . . . . . . . . . . . . . .21
Pilotage du projet avec Scrum . . . . . . . . . . . . . . . . . . . . . . . . . .23

Introduction . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

---

## Introduction

Après avoir présenté le contexte de VECTORYS Tunisie et les limites du processus d’inventaire existant,
ce chapitre formalise le périmètre de la solution*Vectorys Inventaire*. Il transforme les attentes métier
en exigences vérifiables, puis établit leur traduction en cas d’utilisation, en entités du domaine et en
éléments du Product Backlog. Cette démarche garantit qu’une même logique relie la préparation de
l’inventaire, le comptage mobile, le traitement des écarts, le pilotage et les choix d’architecture.

Le chapitre expose d’abord les acteurs et les besoins, puis les deux vues UML globales du système. Il
précise ensuite le pilotage Scrum, l’architecture logique et physique ainsi que l’environnement de travail
retenu. Les exigences sont identifiées par les préfixes RF et RNF afin de maintenir une traçabilité
explicite avec les releases et les sprints détaillés dans les chapitres suivants.

## 2.1Spécification des besoins

La spécification des besoins délimite les responsabilités du système et fournit les critères permettant
d’évaluer les incréments livrés. Elle résulte de l’analyse du processus d’inventaire physique : préparation
d’une mission à partir du stock ERP, répartition des emplacements, comptage terrain, consolidation
des données et traitement des divergences.

## 2.1.1Identification des acteurs

Un acteur représente un rôle extérieur qui échange avec la plateforme. Les composants internes, tels que
SQL Server, SQLite ou SignalR, ne sont donc pas considérés comme des acteurs UML : ils réalisent des
responsabilités techniques à l’intérieur du système. Trois acteurs externes portent les cas d’utilisation
métier de la version réalisée.

Tableau 2.1:Acteurs de la plateforme Vectorys Inventaire

| Acteur | Canal d&#x27;accès | Responsabilités et interactions avec le système |
| --- | --- | --- |
| Superviseur | Portail web Angular | Créer et préparer un inventaire, importer le stock théorique, contrôler les affectations, suivre l&#x27;avancement, analyser les écarts, déclencher un recomptage C2, consolider les résultats et consulter les KPI. |

15

---

Chapitre 2. Spécification et conception globale

| Acteur | Canal d&#x27;accès | Responsabilités et interactions avec le système |
| --- | --- | --- |
| Agent de comptage | Application mobile Flutter | S&#x27;authentifier par code personnel, consulter uniquement les emplacements qui lui sont affectés, scanner les unités de stock, visualiser sa progression et synchroniser les scans conservés hors connexion. |
| ERP / fichier Excel | Système externe source | Fournir au format .xlsx le stock théorique préparé pour l&#x27;inventaire : code emplacement, code unité de stock (US), quantité attendue et code de l&#x27;agent affecté. |

Le rôle techniqueADMINest prévu par les politiques d’autorisation et peut disposer de droits de
supervision étendus, mais aucun cas d’utilisation métier propre à l’administration des comptes n’est
démontré dans le périmètre fonctionnel présenté. L’agent intervient exclusivement sur le périmètre qui
lui a été attribué. L’ERP ne pilote aucune règle métier : il alimente le système au moyen d’un fichier
dont la structure et le contenu sont contrôlés avant toute insertion.

## 2.1.2Spécification des besoins fonctionnels

Les besoins fonctionnels décrivent les services attendus sans imposer leur mécanisme d’implémentation.
Ils sont regroupés suivant le cycle réel d’un inventaire afin d’éviter une liste de fonctions sans lien métier.
Tableau 2.3:Besoins fonctionnels de la plateforme

| ID | Fonction | Description et règle métier principale |
| --- | --- | --- |
| RF-01 | Authentifier les utilisateurs | Permettre au superviseur et à l'administrateur de se connecter par email et mot de passe, et à l’agent par code personnel. Un compte inactif ou des identifiants invalides doivent être rejetés. |
| RF-02 | Contrôler les accès par rôle | Extraire le rôle du jeton JWT, orienter l’utilisateur vers l’espace correspondant à son profil et refuser toute route web, mobile ou API non autorisée. |
| RF-03 | Créer un inventaire | Enregistrer une mission avec une référence unique générée automatiquement, ses dates et son statut initial, puis créer son premier passage C1. |
| RF-04 | Importer le stock théorique | Lire un fichier Excel contenant CodeEmplacement, CodeUS, Qte et CodeAgent, contrôler les colonnes et les valeurs, puis produire un bilan des lignes acceptées ou rejetées. |

16

---

Chapitre 2. Spécification et conception globale

| ID | Fonction | Description et règle métier principale |
| --- | --- | --- |
| RF-05 | Construire et affecter le plan de comptage | Créer, sans doublon, les emplacements et les lignes de stock théorique du passage. Chaque emplacement doit être affecté à un agent existant identifié par CodeAgent. |
| RF-06 | Consulter les tâches terrain | Présenter à l'agent connecté uniquement les emplacements qui lui sont affectés, avec leur état et leur progression, sans dévoiler intitlement les quantités théoriques. |
| RF-07 | Enregistrer un scan | Associer chaque lecture à l'agent, à l'emplacement et à l'instant du comptage. Le code US doit appartenir au périmètre attendu et le quota autorisé ne doit pas être dépassé. |
| RF-08 | Clôturer un emplacement | Recalculer la progression après chaque scan et terminer automatiquement l'emplacement lorsque toutes ses quantités attendues sont couvertes. |
| RF-09 | Assurer le mode hors ligne | Conserver localement les scans lorsque l'API est indisponible, avec un identifiant mobile unique et leur horodatage d'origine. |
| RF-10 | Synchroniser les scans | Transmettre par lot les scans en attente dès le retour du réseau, refuser les doublons à partir de lidentifiant mobile et indiquer à l'agent le résultat de la synchronisation. |
| RF-11 | Suivre l'avancement | Actualiser le portail superviseur lors d'un nouveau scan ou de la fin d'un emplacement et fournir une vue consolidée par inventaire, passage, emplacement et agent. |
| RF-12 | Détecter les écarts | Comparer, à la fin d'un passage, les quantités théoriques et les quantités comptées; créer des écarts de type MANQUANT ou SURPLUS avec leur valeur et leur statut de traitement. |
| RF-13 | Traiter les écarts | Permettre au superviseur de consulter les anomalies et d'appliquer la branche A, qui génère un passage C2 limité aux emplacements nécessitant un recomptage. La branche B, correspondant à un recomptage global, reste un point d'extension. |
| RF-14 | Consolider les résultats | Consolider les résultats du dernier passage pertinent et préparer les données de clôture et d'export. La finalisation complète de ces deux opérations reste un point de durcissement identifié. |
| RF-15 | Consulter les KPI | Calculer à la demande les indicateurs de précision, de performance et de qualité, avec des filtres par inventaire, période et zone. Les KPI sont des agrégats et non des données métier dupliquées. |

Le scénario nominal suit ainsi une chaîne continue : RF-03 à RF-05 préparent le passage C1; RF-06 à
RF-10 exécutent le comptage; RF-11 donne de la visibilité au superviseur; RF-12 à RF-14 transforment

17 les scans en résultat validé; RF-15 exploite ces données pour le pilotage. RF-01 et RF-02 constituent
les fonctions transversales de contrôle d’accès.

## 2.1.3Spécification des besoins non fonctionnels

Les besoins non fonctionnels expriment les qualités attendues de la solution. Ils complètent les règles
métier et servent de critères de conception, de test et de déploiement.

Tableau 2.5:Besoins non fonctionnels de la plateforme

| ID | Qualité | Exigence mesurable ou vérifiable |
| --- | --- | --- |
| RNF-01 | Sécurité | Protéger les échanges par JWT, stocker les mots de passe sous forme hachée, appliquer les droits côté API et ne jamais se fier uniquement au masquage d’une fonction dans l’interface. |
| RNF-02 | Intégrité | Exécuter côté serveur les contrôles d’affectation, de quota, de statut et d’unicité de IdLocalFlutter. Une transaction doit préserver la cohérence des insertions liées à un import ou à une synchronisation. |
| RNF-03 | Résilience | Permettre la poursuite du comptage lors d’une coupure réseau, conserver les scans dans SQLite et reprendre la synchronisation sans perte ni création multiple. |
| RNF-04 | Performance | Utiliser des traitements par lots pour l’import et la synchronisation, indexer les clés de recherche fréquentes et maintenir une réponse immédiate de l’interface mobile après une lecture. |
| RNF-05 | Utilisabilité | Limiter le nombre d’actions pendant la boucle de scan, afficher un retour clair après chaque lecture et rendre visibles les états « en attente », « synchronisé », « terminé » et « en erreur ». |
| RNF-06 | Maintenabilité | Séparer Domain, Application, Infrastructure et API; isoler les interfaces Angular et Flutter; centraliser les règles avec CQRS/MediatR et valider les entrées avec FluentValidation. |
| RNF-07 | Traçabilité | Horodater les créations et les scans, conserver l’auteur des opérations sensibles et fournir des résultats rattachables à un inventaire, un passage, un emplacement et un agent. |

18

---

Chapitre 2. Spécification et conception globale

| ID | Qualité | Exigence mesurable ou vérifiable |
| --- | --- | --- |
| RNF-08 | Déployabilité | Pouvoir publier l&#x27;API et le portail sur le réseau local VECTORYS, paramétrer l&#x27;adresse de l&#x27;API dans l&#x27;APK, configurer CORS et SignalR et documenter les endpoints avec Swagger. |

## 2.2Diagramme de cas d’utilisation global

Le diagramme de la figure 2.1 représente le périmètre fonctionnel confirmé par les chapitres de release
et par les interfaces réalisées. Il distingue trois acteurs : le superviseur, l’agent de comptage et le
système ERP représenté par le fichier Excel d’import. Les relations «include» isolent un comportement
obligatoire du scénario principal, tandis que « extend » représente une variante conditionnelle, comme
le comptage hors connexion ou le recomptage ciblé C2.

19

---

Lancer la Branche A :
2.1 : Diagramme de cas d’utilisation global de Vectorys Inventaire

Chapitre 2. Spécification et conception globale

---

Le scénario principal débute par la création d’un inventaire et la génération automatique de son
passage C1. Le superviseur importe ensuite le fichier ERP; la validation crée le stock théorique, les
emplacements et les affectationsCodeAgent. L’agent consulte uniquement son périmètre, scanne les
US et reçoit la progression calculée par le serveur jusqu’à la clôture automatique de l’emplacement. En
cas de coupure, il poursuit le comptage dans SQLite puis synchronise les scans avec déduplication par
IdLocalFlutter. Le superviseur suit les agents en temps réel, détecte les écartsMANQUANT/SURPLUS,
déclenche si nécessaire la Branche A et consulte les KPI. Les activités techniques de test et de
déploiement ne figurent pas dans ce diagramme métier; la Branche B, la clôture définitive et l’export
complet restent explicitement signalés comme consolidation ou évolution.

## 2.3Diagramme de classes global

Le modèle de domaine de la figure 2.2 traduit les concepts persistants nécessaires aux cas d’utilisation.
Il ne reproduit pas les DTO, contrôleurs ou classes techniques de la Clean Architecture. Les clés
étrangères sont indiquées pour rendre les dépendances explicites, mais les associations et multiplicités
restent la référence UML pour comprendre le cycle de vie des objets.

21

---

Utilisateur
- id : Guid
- codeAgent : string [0..1]
- nomComplet : string
- email : string [0..1]
- motDePasseHash : string
- role : RoleUtilisateur
- actif : bool
- dateCreation : DateTime
+ peutSeConnecter() : bool

Inventaire
- id : Guid
- reference : string {unique}
- libelle : string
- dateCreation : DateTime
- dateCloture : DateTime [0..1]
- statut : StatutInventaire
- creeParId : Guid

contenir
1 1.*

Passage
- id : Guid
- inventaireId : Guid
- numero : int
- type : TypePassage {C1,C2}
- dateDebut : DateTime
- dateFin : DateTime [0..1]
- statut : StatutPassage

générer
1 0.*

Ecart
- id : Guid
- passageId : Guid
- stockTheoriqueld : Guid
- quantiteComptee : int
- difference : int
- type : TypeEcart
{MANQUANT,SURPLUS}
- statut : StatutEcart
- dateDetection : DateTime

marquerTraite()

Scan
- id : Guid
- emplacementId : Guid
- agentId : Guid
- codeUS : string
- horodatage : DateTime
- idLocalFlutter : string [0..1]
{unique}
- dateSynchronisation : DateTime [0..1]
+ estSynchronise() : bool

regrouper
1 0.*

Emplacement
- id : Guid
- passageId : Guid
- agentAssigneId : Guid
- code : string
- zone : string [0..1]
- statut : StatutEmplacement
- dateFinComptage : DateTime [0..1]

contenir
1 1.*

StockTheorique
- id : Guid
- emplacementId : Guid
- codeUS : string
- quantiteTheorique : int
+ quantiteScannee() : int
+ quotaAtteint() : bool

recevoir
1 0.*

FIGURE 2.2 : Diagramme de classes global du domaine métier

---

Les compositions expriment les dépendances de cycle de vie : un passage n’existe que dans un inventaire,
un emplacement appartient à un passage et ses lignes théoriques structurent le comptage. Un utilisateur
peut créer plusieurs inventaires, être affecté à plusieurs emplacements et produire plusieurs scans.
L’identifiantidLocalFlutter, lorsqu’il existe, est unique; il assure l’idempotence de la synchronisation.
L’entitéEcartconserve la référence au passage contrôlé et à la ligne théorique concernée afin de justifier
le calcul et de préparer un éventuel C2. Les KPI ne figurent pas dans le modèle, puisqu’ils sont calculés
à partir des stocks, scans et écarts.

## 2.4Pilotage du projet avec Scrum

Le caractère multi-canal du projet et la nécessité de valider rapidement les règles d’entrepôt ont conduit
à retenir Scrum. Le backlog a été ordonné de manière à livrer d’abord un socle exécutable, puis un
parcours métier complet, avant d’ajouter la résilience, l’analyse et la mise en service. Les itérations
durent une à deux semaines selon la charge et se terminent par un incrément démontrable.

## 2.4.1Équipe et rôles Scrum

L’équipe étant réduite dans le cadre du PFE, une adaptation pragmatique des rôles a été appliquée
tout en conservant la responsabilité propre à chacun.

Tableau 2.7:Répartition des responsabilités du projet Scrum

| Rôle Scrum | Responsable | Responsabilités dans le projet |
| --- | --- | --- |
| Product Owner | Encadrant professionnel / représentant métier | Clarifier les règles d’inventaire, ordonner les besoins selon leur valeur opérationnelle, fournir les exemples de fichiers et accepter les incréments lors des revues. |
| Scrum Master | Élève ingénieur | Préparer les planifications et revues, tenir le tableau de suivi, rendre visibles les obstacles et adapter l&#x27;organisation sans modifier unilatéralement les priorités métier. |
| Équipe de développement | Élève ingénieur, avec appui ponctuel des encadrants | Concevoir le modèle et l&#x27;architecture, développer l&#x27;API, le portail et l&#x27;application mobile, tester chaque incrément, documenter les décisions et estimer le travail restant. |

23

---

Chapitre 2. Spécification et conception globale

| Rôle Scrum | Responsable | Responsabilités dans le projet |
| --- | --- | --- |
| Partie prenante académique | Encadrante académique | Examiner la qualité méthodologique, la cohérence de la conception et la conformité des livrables aux exigences d&#x27;un projet de fin d&#x27;études. |

Le Sprint Planning sélectionne les éléments prêts et formule un objectif d’itération. Un suivi court et
régulier remplace le Daily Scrum formel d’une équipe nombreuse. La Sprint Review sert à confronter
l’incrément au processus réel, tandis que la rétrospective identifie les améliorations de méthode, par
exemple le renforcement des contrôles d’import ou le déplacement d’une règle critique vers l’API.

## 2.4.2Product Backlog

Le Product Backlog ci-dessous traduit les besoins fonctionnels en User Stories ordonnées. La priorité
combine valeur métier, risque et dépendances. L’estimation en points représente un effort relatif
intégrant développement, tests et documentation; elle n’est pas une durée en jours.

Tableau 2.9:Product Backlog global ordonné

| ID | User Story | RF | Prio. | Points | Critère d&#x27;acceptation synthétique |
| --- | --- | --- | --- | --- | --- |
| PB-01 | En tant qu&#x27;utilisateur, je veux m&#x27;authentifier selon mon profil afin d&#x27;accéder uniquement à mon espace. | RF-01 | Haute | 5 | Jeton valide émis; compte inactif et route hors rôle rejetés. |
| PB-02 | En tant qu&#x27;utilisateur authentifié, je veux être orienté vers l&#x27;espace correspondant à mon rôle afin de ne voir que les fonctions autorisées. | RF-02 | Haute | 3 | Route hors rôle refusée et redirection correcte vers le portail web ou l&#x27;application mobile. |
| PB-03 | En tant que superviseur, je veux créer un inventaire et son C1 afin de démarrer une mission traçable. | RF-03 | Haute | 5 | Référence unique et passage C1 créés dans une même opération cohérente. |
| PB-04 | En tant que superviseur, je veux importer le fichier ERP afin d&#x27;alimenter le stock à compter. | RF-04 | Haute | 8 | Colonnes contrôlées et rapport précis des lignes invalides. |
| PB-05 | En tant que superviseur, je veux affecter chaque emplacement à un agent afin d&#x27;éviter les zones ambigués. | RF-05 | Haute | 5 | Emplacements, stocks et affectations créés sans doublon. |
| PB-06 | En tant qu&#x27;agent, je veux voir mes emplacements afin d&#x27;exécuter seulement mon périmètre. | RF-06 | Haute | 3 | Aucun emplacement d&#x27;un autre agent n&#x27;est retourné. |
| PB-07 | En tant qu&#x27;agent, je veux scanner en boucle afin de compter rapidement les unités de stock. | RF-07 | Haute | 8 | Scan valide enregistré; US inconnu ou quota dépassé refusé. |

24

---

Chapitre 2. Spécification et conception globale

| ID | User Story | RF | Prio. | Points | Critère d&#x27;acceptation synthétique |
| --- | --- | --- | --- | --- | --- |
| PB-08 | En tant qu&#x27;agent, je veux voir la progression et la clôture automatique afin de savoir quand changer d&#x27;emplacement. | RF-08 | Haute | 5 | Progression recalculée et statut terminé déclenché au quota global. |
| PB-09 | En tant qu&#x27;agent, je veux continuer hors connexion afin de ne pas interrompre le comptage. | RF-09 | Haute | 8 | Scans horodatés et conservés dans SQLite sans réseau. |
| PB-10 | En tant qu&#x27;agent, je veux synchroniser les scans en attente afin de restituer le travail hors ligne. | RF-10 | Haute | 8 | Lot traité, doublons ignorés et état local mis à jour. |
| PB-11 | En tant que superviseur, je veux suivre l&#x27;opération en temps réel afin de détecter les zones en retard. | RF-11 | Haute | 5 | Nouveau scan et fin d&#x27;emplacement visibles sans rechargement manuel. |
| PB-12 | En tant que superviseur, je veux détecter les écarts afin d&#x27;identifier les stocks non conformes. | RF-12 | Haute | 8 | MANQUANT et SURPLUS calculés sur un jeu de référence. |
| PB-13 | En tant que superviseur, je veux déclencher un C2 ciblé afin de vérifier les seuls emplacements en écart. | RF-13 | Haute | 8 | Nouveau passage limité aux emplacements sélectionnés et lié au même inventaire. |
| PB-14 | En tant que superviseur, je veux consolider les résultats C1/C2 afin de préparer la finalisation de l’inventaire. | RF-14 | Moy. | 5 | Les données du dernier passage pertinent sont rapprochées du stock théorique sans incohérence. |
| PB-15 | En tant que superviseur, je veux consulter des KPI filtrables afin de piloter la performance de l’inventaire. | RF-15 | Moy. | 8 | Agrégats vérifiés et filtre invaient, période et zone opérationnels. |
| PB-16 | En tant qu&#x27;exploitant, je veux une version documentée et déployable sur le LAN afin de valider la solution en contexte interne. | RNF-08 | Moy. | 8 | API, portail et APK communiquent sur le réseau local; Swagger accessible. |

La Definition of Done commune impose une implémentation compilable, une validation des entrées, un
contrôle des droits, des tests du scénario nominal et des principaux rejets, ainsi qu’une démonstration
depuis le client concerné. Cette règle évite de déclarer terminé un élément dont seule l’interface ou
seule l’API serait disponible.

## 2.4.3Répartition des releases et sprints

Avant le démarrage des itérations fonctionnelles, une phase préparatoire met en place le socle technique :
création des projets, configuration des outils, initialisation de la persistance et validation de la
compilation. Cette étape n’est pas comptabilisée comme un sprint, car elle ne produit pas encore
d’incrément métier destiné aux utilisateurs.

25

---

Les seize éléments du backlog sont ensuite répartis équitablement en trois releases. Chacune regroupe
trois sprints, soit neuf sprints numérotés de S1 à S9. Cette organisation conserve l’ordre des dépendances
fonctionnelles tout en donnant à chaque release un périmètre homogène et un résultat démontrable.
Tableau 2.11:Répartition des releases et des sprints

| Release | Sprint | Objectif du sprint | Périmètre principal | Backlog exigences |
| --- | --- | --- | --- | --- |
| R1 | S1 | Sécuriser les accès | Authentification web/mobile, JWT, hachage, rôles et protection des routes. | PB-01, PB-02/RF-01-02 |
| R1 | S2 | Préparer un inventaire | Crédation de la mission et de C1, import Excel, stocks théoriques et affectations. | PB-03-05/RF-03-05 |
| R1 | S3 | Réaliser le comptage connecté | Liste des tâches, scan, quota, progression, clôture automatique et SignalR. | PB-06-08, PB-11/RF-06-08, RF-11 |
| R2 | S4 | Rendre le mobile résilient | SQLite, identifiant local, file d'attente, endpoint batch et déduplication. | PB-09-10/RF-09-10 |
| R2 | S5 | Détecter et traiter les écarts | Calcul MANQUANT/SURPLUS, consultation et création d&#x27;un C2 cible. | PB-12-13/RF-12-13 |
| R2 | S6 | Consolider l’inventaire | Consolidation C1/C2, premières fonctions de clôture et d&#x27;export, préparation de l&#x27;extension Branche B. | PB-14/RF-14 |
| R3 | S7 | Fournir le pilotage KPI | Agrégats, filtres, graphiques Angular et optimisation des requêtes. | PB-15/RF-15 |
| R3 | S8 | Stabiliser et documenter | Scénarios Swagger/Postman, jeux d&#x27;essai, tests de bout en bout et corrections. | Tous RF/RNF-01-07 |
| R3 | S9 | Préparer la mise en service | Publication locale, configuration réseau, CORS/SignalR et APK paramétré. | PB-16/RNF-08 |

Ce découpage respecte les dépendances du domaine : il serait impossible de tester le scan avant

26 l’existence d’un passage et de ses affectations, ou de calculer un écart avant de disposer de scans
consolidés. La release R1 fournit le premier parcours connecté complet, la release R2 garantit sa
résilience et transforme les comptages en résultats consolidés, puis la release R3 apporte le pilotage, la
validation et la mise en service.

## 2.5Architecture de l’application

L’architecture répond à deux préoccupations complémentaires. La vue logique montre comment le
code sépare les responsabilités; la vue physique montre où les composants s’exécutent et comment ils
communiquent dans le réseau VECTORYS.

## 2.5.1Architecture logique de l’application

La solution adopte une architecture three-tier à l’échelle du système et une Clean Architecture
à l’intérieur du backend. Les deux vues ne se contredisent pas : le three-tier décrit les niveaux
présentation, traitement et données, tandis que la Clean Architecture organise les dépendances du
code de l’API.

Tableau 2.13:Couches logiques et responsabilités

| Niveau logique | Composants | Responsabilités |
| --- | --- | --- |
| Présentation | Angular 19 et Flutter | Afficher les données, collecter les actions, conserver l&#x27;état d&#x27;interface et appeler l&#x27;API. Flutter gère en plus la file SQLite, mais ne décide pas des quotas ou des droits. |
| API / exposition | Contrôleurs REST, JWT, SignalR, Swagger | Authentifier la requête, appliquer les politiques de rôle, exposer les cas d&#x27;utilisation et diffuser les événements utiles au portail. |
| Application | Commandes et requêtes CQRS, handlers MediatR, FluentValidation | Orchestrer un cas d&#x27;utilisation, valider les données d&#x27;entrée et coordonner les interfaces de persistance et de services. |
| Domaine | Entités, énumérations et règles métier | Porter les invariants indépendants des frameworks : statuts, quotas, progression, clôture et classification des écarts. |

27

---

Chapitre 2. Spécification et conception globale

| Niveau logique | Composants | Responsabilités |
| --- | --- | --- |
| Infrastructure | EF Core, ClosedXML, repositories, service JWT | Implémenter l'accès SQL Server, les transactions, l'import/export Excel et les services techniques définis par la couche Application. |

Les dépendances du backend sont orientées vers le domaine : Domain ne dépend d’aucun framework,
Application dépend de Domain, et Infrastructure ainsi qu’API implémentent ou consomment les
contrats applicatifs. Cette organisation permet de tester les règles d’inventaire sans interface graphique
et empêche leur duplication entre Angular et Flutter.

## 2.5.2Architecture physique de l’application

Le déploiement cible le réseau local de l’entreprise. Le poste du superviseur accède au portail Angular
par le LAN. Les terminaux Android rejoignent l’API par le Wi-Fi de l’entrepôt; en cas de perte de
couverture, ils écrivent temporairement dans leur base SQLite. Le serveur applicatif héberge l’API
ASP.NET Core et le portail compilé, tandis que SQL Server centralise les données métier.

Figure 2.3:Architecture physique de déploiement sur le réseau local

La sécurité et la cohérence restent portées par l’API, y compris lors de la reprise après une
coupure. SQLite ne constitue pas une seconde base métier : il s’agit d’une file locale durable, vidée
progressivement après acceptation des lots par le serveur. Cette distinction évite les conflits de deux
sources de vérité.

28

---

## 2.6Environnement de travail

L’environnement de travail a été choisi pour couvrir la modélisation, le développement
multi-plateforme, la persistance, les tests d’API et le suivi incrémental. Les versions indiquées
correspondent au socle retenu pour le projet.

## 2.6.1Langage de modélisation

UML 2 est utilisé comme langage de modélisation commun. Le diagramme de cas d’utilisation délimite
les services perçus par les acteurs; le diagramme de classes structure le domaine; les diagrammes
de séquence des chapitres de release détaillent les collaborations pour l’authentification, le scan, la
synchronisation et le traitement des écarts. UML est employé comme outil de communication et de
validation, non comme une documentation exhaustive de chaque classe technique.

Les conventions suivies sont les associations nommées, les multiplicités, la composition lorsque le cycle
de vie est dépendant, et les relations*include*/*extend*seulement lorsqu’elles expriment respectivement
un sous-comportement obligatoire ou une variante conditionnelle. Cette discipline maintient la
correspondance entre les diagrammes et le modèle implémenté.

## 2.6.2Environnement matériel

Le développement et la validation reposent sur quatre catégories de matériel correspondant aux
conditions réelles d’utilisation.

Tableau 2.15:Environnement matériel du projet

| Équipement | Usage | Contraintes prises en compte |
| --- | --- | --- |
| Poste de développement | Compilation .NET, Angular et Flutter, émulation et exécution des outils de test | Capacité suffisante pour exécuter simultanément l&#x27;API, SQL Server, le navigateur et un émulateur Android. |
| Poste superviseur | Accès au portail web et aux tableaux de bord | Navigateur moderne, connexion au LAN et affichage adapté aux tableaux de suivi. |
| Terminal Android/PDA | Authentification agent, scan de codes-barres et stockage SQLite | Mobilité, lecture rapide, autonomie, retour visuel immédiat et fonctionnement dans les zones Wi-Fi irrégulières. |

29

---

Chapitre 2. Spécification et conception globale

| Équipement | Usage | Contraintes prises en compte |
| --- | --- | --- |
| Serveur local VECTORYS | Hébergement de l&#x27;API, du portail et accès à SQL Server | Adresse IP privée stable, ports contrôlés, sauvegarde des données et disponibilité pendant l&#x27;opération d&#x27;inventaire. |
| Réseau LAN / Wi-Fi | Échanges web, API et SignalR | Couverture variable dans l&#x27;entrepôt, ce qui justifie la file hors ligne et la reprise idempotente. |

## 2.6.3Environnement logiciel

Tableau 2.17:Environnement logiciel du projet

| Catégorie | Outil / technologie | Utilisation dans le projet |
| --- | --- | --- |
| Backend | C#, ASP.NET Core 9 | API REST, injection de dépendances, authentification, autorisation et hub SignalR. |
| Architecture applicative | MediatR, FluentValidation | Mise en œuvre de CQRS, découplage des cas d&#x27;utilisation et validation des commandes et requêtes. |
| Persistance centrale | Microsoft SQL Server, Entity Framework Core 9 | Modèle relationnel, migrations, transactions et requêtes d&#x27;agrégation. |
| Portail web | Angular 19, TypeScript, Chart.js | Interfaces de préparation et de supervision, traitement des écarts et visualisation des KPI. |
| Application mobile | Flutter, Dart, SQLite | Interface terrain Android, scan, progression et file locale hors connexion. |
| Échanges bureautiques | ClosedXML | Lecture contrôle du stock théorique et génération des exports Excel. |
| Test et documentation API | Swagger / OpenAPI, Postman | Exploration des contrats REST, scénarios d&#x27;intégration et constitution de jeux de validation reproductibles. |
| Gestion de versions | Git | Historisation du code, organisation des changements par fonctionnalité et sécurisation des incréments. |

---

Chapitre 2. Spécification et conception globale

| Catégorie | Outil/technologie | Utilisation dans le projet |
| --- | --- | --- |
| Modélisation et rédaction | UML, LaTeX | Formalisation des vues de conception et production du rapport technique. |

## Conclusion

Ce chapitre a établi une spécification globale cohérente de*Vectorys Inventaire*. Les trois acteurs, les
quinze besoins fonctionnels et les huit qualités attendues sont reliés aux cas d’utilisation, aux entités du
domaine et aux éléments du Product Backlog. Après une phase préparatoire hors sprint, le découpage
en trois releases et neuf sprints respecte les dépendances fonctionnelles, depuis la sécurisation des accès
jusqu’au déploiement local.

Les diagrammes globaux fixent désormais un référentiel commun pour la suite du rapport : les passages
C1/C2, les affectations, les scans, les écarts et la clôture possèdent une traduction stable dans le modèle.
Les chapitres suivants peuvent ainsi détailler chaque release sans réintroduire le périmètre général ni
modifier les règles établies ici.

---

*Chapitre 3*

Release 1 : sécurité, préparation et

---

## Plan

Introduction . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 34

1 Phase préparatoire : initialisation du socle technique . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 34

2 Sprint 1 : authentification et autorisation . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 35

3 Sprint 2 : inventaire, import Excel et assignation . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 38

4 Sprint 3 : scan, quotas et temps réel . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 41

Conclusion . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

---

## Introduction

La première release construit le premier parcours métier complet de la plateforme*Vectorys Inventaire*.
Elle intervient après une phase préparatoire consacrée à l’installation du socle technique, puis regroupe
trois sprints fonctionnels : sécurisation des accès, préparation de l’inventaire et comptage connecté.
À son terme, un superviseur peut créer une mission et affecter les emplacements, tandis qu’un agent
authentifié peut réaliser le comptage depuis l’application mobile.

## 3.1Phase préparatoire : initialisation du socle technique

Cette phase précède les sprints Scrum. Elle ne correspond pas à un incrément fonctionnel destiné aux
utilisateurs et n’est donc pas numérotée comme un sprint. Son rôle est de rendre l’environnement
cohérent, compilable et suffisamment stable pour accueillir les fonctionnalités métier sans reprendre
ultérieurement les fondations du projet.

## 3.1.1Mise en place de l’environnement de développement

Le poste de développement a été préparé pour exécuter simultanément les trois applications et leurs
services associés. Le SDK .NET, Node.js et Angular CLI, Flutter, SQL Server ainsi que Git ont été
configurés et vérifiés. Cette préparation a également porté sur les fichiers de configuration propres à
chaque environnement, les chaînes de connexion et les paramètres d’accès à l’API depuis les clients
web et mobile.

## 3.1.2Création et organisation des projets

La solution backend a été initialisée avec les projets Domain, Application, Infrastructure et API
conformément à l’architecture logique définie au chapitre précédent. Les projets Angular et Flutter ont
été créés séparément afin de préserver l’indépendance de leurs cycles de compilation et de déploiement.
Le dépôt Git a ensuite été structuré avec des conventions communes de nommage et d’organisation
des changements.

## 3.1.3Initialisation de la persistance

La connexion à SQL Server, leDbContextEntity Framework Core et les premières migrations ont
été préparés. Les entités nécessaires à la construction progressive du domaine ont été introduites sans
implémenter prématurément les règles des futurs sprints. La baseVectorysDBconstitue ainsi la source
de vérité centrale, tandis que SQLite est réservé à la file locale de synchronisation du client mobile.

---

## 3.1.4Préparation des services transversaux

Les dépendances techniques nécessaires au projet ont été ajoutées : MediatR pour l’organisation des cas
d’utilisation, FluentValidation pour le contrôle des entrées, SignalR pour les notifications, ClosedXML
pour les échanges Excel et Swagger pour l’exploration de l’API. L’injection de dépendances, CORS et les
principaux fichiers de configuration ont également été préparés, sans anticiper la logique fonctionnelle
des sprints.

## 3.1.5Synthèse des éléments préparés

Tableau 3.1:Synthèse de l’initialisation du socle technique

| Élément préparé | Action réalisée | Résultat obtenu |
| --- | --- | --- |
| Backend | Création des projets Domain, Application, Infrastructure et API | Solution .NET compilable |
| Clients | Initialisation des applications Angular et Flutter | Portail web et application mobile exécutables |
| Persistance | Configuration d&#x27;EF Core, SQL Server et de la migration initiale | Base VectorysDB accessible |
| Services techniques | Ajout de MediatR, FluentValidation, SignalR, ClosedXML et Swagger | Dépendances prêtes pour les sprints fonctionnels |
| Organisation | Mise en place du dépôt Git et des conventions de nommage | Socle de développement partagé et traçable |

La validation de cette phase a consisté à compiler les projets, démarrer l’API, appliquer la migration
initiale et lancer les deux clients. Ces vérifications confirment que le socle est opérationnel avant
l’ouverture du Sprint 1.

## 3.2Sprint 1 : authentification et autorisation

## 3.2.1Objectifs du Sprint 1

Le Sprint 1 vise à sécuriser l’accès à la plateforme. Il couvre la connexion des superviseurs,
administrateurs et agents, ainsi que la protection des endpoints API selon les rôles.

---

## 3.2.2Backlog du Sprint 1

–Authentifier superviseur et administrateur par email et mot de passe.

–Authentifier l’agent par code personnel.

–Générer un jeton JWT avec rôle, identifiant utilisateur et durée de validité.

–Rejeter les comptes inactifs.

–Protéger les routes web, mobile et API par rôle.

## 3.2.3Planification des tâches du Sprint 1

Tableau 3.3:Planification des tâches du Sprint 1

| Tache | Description | Livrable |
| --- | --- | --- |
| Service JWT | Générer et valider les tokens selon le rôle | JwtService |
| Endpoint login | Exposer POST /api/auth/login pour web et mobile | AuthController |
| Policies | Restreindre les endpoints sensibles | Autorisation API |
| Interfaces | Créer les écrans de connexion Angular et Flutter | LoginWeb/Mobile |

## 3.2.4Conception du Sprint 1

Le cas d’utilisation central est « S’authentifier ». Deux variantes sont prévues : email et mot de passe
pour le superviseur, code personnel pour l’agent. Le diagramme de séquence associé implique le client,
l’API Auth, la base de données et le service JWT.

## 3.2.5Réalisation du Sprint 1

Le sprint livre le contrôleur d’authentification, les DTO de connexion, le hachage des mots de passe,
la génération JWT, les politiques de rôle et les écrans de connexion Angular et Flutter.

La figure 3.1 présente l’interface web mise à la disposition du superviseur. Elle permet de saisir l’adresse
électronique professionnelle et le mot de passe, tout en rappelant le caractère sécurisé et réservé de
l’accès au portail.

---

Figure 3.1:Interface d’authentification du superviseur

## 3.2.6Tests du Sprint 1

Les tests couvrent la connexion valide, les identifiants invalides, le compte inactif, l’accès refusé à un
endpoint hors rôle et la durée de validité différenciée du jeton.

## 3.2.7Revue et rétrospective du Sprint 1

La revue valide l’accès sécurisé aux espaces superviseur et agent. La rétrospective montre que la
séparation des modes de connexion réduit les erreurs d’usage entre web et mobile.

---

## 3.3Sprint 2 : inventaire, import Excel et assignation

## 3.3.1Objectifs du Sprint 2

L’objectif du Sprint 2 est de permettre au superviseur de préparer un inventaire exploitable sur le
terrain. À la fin du sprint, les agents disposent d’emplacements assignés issus du fichier Excel.

## 3.3.2Backlog du Sprint 2

–Créer un inventaire avec référence automatique.

–Générer automatiquement le passage C1.

–Importer un fichier Excel contenant CodeEmplacement, CodeUS, Qte et CodeAgent.

–Valider les colonnes obligatoires et les agents existants.

–Créer les emplacements, stocks théoriques et affectations agent-emplacement.

–Produire un rapport d’import exploitable.

## 3.3.3Planification des tâches du Sprint 2

Tableau 3.5:Planification des tâches du Sprint 2

| Tache | Description | Livrable |
| --- | --- | --- |
| Création inventaire | Générer la référence et le passage C1 | Endpoint inventaires |
| Lecture Excel | Lire les lignes via ClosedXML | ImportService |
| Validation | Contrôler CodeAgent, colonnes et cohérence des données | Validateurs |
| Persistance batch | Créer emplacements et stock théorique | Données importées |
| Interface web | Fournir l&#x27;écran de création et d&#x27;import | UI Angular |

## 3.3.4Conception du Sprint 2

La conception relie Inventaire, Passage, Emplacement, StockTheorique et Utilisateur. Le passage C1
est généré automatiquement afin de standardiser le démarrage du cycle. La colonne CodeAgent devient
une règle métier obligatoire pour éviter les affectations ambiguës.

---

## 3.3.5Réalisation du Sprint 2

La réalisation livre les endpoints POST /api/inventaires et POST /api/passages/{id}/import-stock,
le service ClosedXML, les validations d’import, les insertions batch et l’écran Angular de préparation.
La figure 3.2 illustre l’écran Angular utilisé par le superviseur pour initialiser une mission d’inventaire.
Cette interface centralise les informations générales, le choix du passage C1 ou C2, la sélection des
agents et le dépôt du fichier Excel contenant le stock théorique et les affectations parCodeAgent.

---

[Image: Im10]

Chapitre 3. Release 1 : sécurité, préparation et comptage connecté

---

## 3.3.6Tests du Sprint 2

Les tests vérifient la création d’inventaire, la génération C1, l’import valide, le rejet des fichiers
incomplets, le rejet d’un CodeAgent inconnu et la création correcte des affectations.

## 3.3.7Revue et rétrospective du Sprint 2

La revue confirme que le superviseur peut préparer un inventaire complet depuis le web. La rétrospective
met en évidence l’importance de messages d’erreur explicites pour les imports Excel.

## 3.4Sprint 3 : scan, quotas et temps réel

## 3.4.1Objectifs du Sprint 3

L’objectif du Sprint 3 est de rendre le comptage terrain utilisable sur mobile et observable par le
superviseur en temps réel.

## 3.4.2Backlog du Sprint 3

–Afficher les emplacements assignés à l’agent connecté.

–Enregistrer un scan avec horodatage, agent, passage et emplacement.

–Vérifier le quota théorique par code US.

–Clôturer automatiquement l’emplacement lorsque tous les US sont couverts.

–Notifier le superviseur via SignalR.

## 3.4.3Planification des tâches du Sprint 3

Tableau 3.7:Planification des tâches du Sprint 3

| Tâche | Description | Livrable |
| --- | --- | --- |
| Liste agent | Filtrer les emplacements par AgentAssigneId | Endpoint mes-emplacements |
| Enregistrement scan | Créer les scans côté API | ScansController |
| Quota | Comparer scanné et QteTheorique | Service de complétion |
| Temps réel | Diffuser NouveauScan et FinComptage | Hub SignalR |
| Mobile | Construire l&#x27;écran de scan et de progression | UI Flutter |

---

## 3.4.4Conception du Sprint 3

Le diagramme de séquence part du mobile vers l’API Scans, puis vers le service de complétion et SQL
Server. Si l’emplacement est terminé, SignalR notifie le superviseur. Les règles de quota restent côté
serveur pour garantir l’intégrité.

## 3.4.5Réalisation du Sprint 3

Le sprint livre la liste des emplacements assignés, la boucle de scan Flutter, l’endpoint POST
/api/scans, les contrôles de quota, l’auto-clôture et les notifications temps réel.

## 3.4.6Tests du Sprint 3

Les tests couvrent le scan valide, le dépassement de quota, l’affichage de progression, la clôture
automatique et la réception des notifications sur le portail superviseur.

## 3.4.7Revue et rétrospective du Sprint 3

La revue valide la boucle de comptage mobile. La rétrospective confirme que le contrôle serveur des
quotas est indispensable pour éviter les incohérences terrain.

## Conclusion

La Release 1 fournit le premier parcours connecté complet de*Vectorys Inventaire*. Après l’initialisation
du socle, elle sécurise les accès, prépare le passage C1 à partir du fichier Excel et rend le comptage
mobile observable en temps réel. Ces résultats constituent la base nécessaire à l’ajout de la résilience
hors ligne et au traitement des écarts dans la release suivante.

---

*Chapitre 4*

# Release 2 : résilience et traitement des écarts

---

## Plan

Introduction . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 45

1 Sprint 4 : mode hors ligne et synchronisation batch . . . . . . . . . . . . . . . . . . . . . . . . . . . 45

2 Sprint 5 : détection et traitement des écarts . . . . . . . . . . . . . . . . . . . . . . . . . . . . 46

3 Sprint 6 : clôture, consolidation et export . . . . . . . . . . . . . . . . . . . . . . . . . . . . 47

Conclusion . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 48

---

## Introduction

La deuxième release consolide le parcours de comptage livré précédemment. Elle garantit d’abord la
continuité du travail dans les zones où la connexion Wi-Fi est irrégulière, puis transforme les scans
collectés en écarts exploitables. Enfin, elle prépare la consolidation des passages C1 et C2 ainsi que la
clôture de l’inventaire. Cette release regroupe les Sprints 4, 5 et 6.

## 4.1Sprint 4 : mode hors ligne et synchronisation batch

## 4.1.1Objectifs du Sprint 4

L’objectif du Sprint 4 est de garantir la continuité du comptage dans les zones d’entrepôt où la
connectivité Wi-Fi est instable.

## 4.1.2Backlog du Sprint 4

–Stocker les scans localement en SQLite en mode hors ligne.

–Attribuer un identifiant local unique aux scans Flutter.

–Synchroniser les scans par batch au retour réseau.

–Dédupliquer les scans côté API.

–Afficher l’état de synchronisation à l’agent.

## 4.1.3Planification des tâches du Sprint 4

Tableau 4.1:Planification des tâches du Sprint 4

| Tache | Description | Livrable |
| --- | --- | --- |
| SQLite | Créer la file locale de scans | Base locale |
| Repository offline | Basculer automatiquement selon l&#x27;état réseau | Service mobile |
| Batch API | Recevoir plusieurs scans en une requête | Endpoint batch |
| Déduplication | Utiliser IdLocalFlutter pour éviter les doublons | Règle API |
| Feedback agent | Afficher synchronisé / en attente | UI Flutter |

---

## 4.1.4Conception du Sprint 4

La conception introduit le flux Mobile offline*→*SQLite*→*file de synchronisation*→*API batch*→*
SQL Server. L’API conserve les règles métier afin que le mode hors ligne ne contourne pas les quotas.

## 4.1.5Réalisation du Sprint 4

La réalisation comprend le stockage local SQLite, le service OfflineSyncService, l’endpoint POST
/api/scans/batch, l’identifiant IdLocalFlutter et les états visuels de synchronisation.

## 4.1.6Tests du Sprint 4

Les tests vérifient la création de scans hors ligne, la synchronisation après reconnexion, l’absence de
doublons et la conservation des horodatages.

## 4.1.7Revue et rétrospective du Sprint 4

La revue valide la robustesse terrain. La rétrospective montre que le fonctionnement hors ligne doit
rester transparent pour l’agent tout en demeurant strict côté serveur.

## 4.2Sprint 5 : détection et traitement des écarts

## 4.2.1Objectifs du Sprint 5

L’objectif du Sprint 5 est de comparer le stock théorique et le stock scanné afin d’identifier les anomalies
et de lancer un recomptage ciblé.

## 4.2.2Backlog du Sprint 5

–Détecter les écarts MANQUANT et SURPLUS.

–Afficher les écarts par inventaire.

–Fournir des agrégats au superviseur.

–Appliquer la Branche A par création d’un C2 ciblé.

## 4.2.3Planification des tâches du Sprint 5

Tableau 4.3:Planification des tâches du Sprint 5

| Tâche | Description | Livrable |
| --- | --- | --- |
| Détection | Comparer théorique et scanné | Service écarts |

---

Chapitre 4. Release 2 : résilience et traitement des écarts

| Tâche | Description | Livrable |
| --- | --- | --- |
| Persistance | Créer les lignes Ecart | Table Ecarts |
| Branche A | Créer C2 ciblé et cloner les emplacements | Workflow C2 |
| Interface | Afficher la liste des écarts | UI Angular |

## 4.2.4Conception du Sprint 5

La conception introduit l’entité Ecart avec son type et son statut. Le superviseur choisit la stratégie
de traitement, tandis que le système calcule les écarts de manière automatique.

## 4.2.5Réalisation du Sprint 5

Le sprint livre les endpoints de détection, la liste des écarts, l’application de la Branche A, la création
du C2 ciblé et l’interface Angular associée.

## 4.2.6Tests du Sprint 5

Les tests valident les cas MANQUANT, SURPLUS, absence d’écart, création du C2 et changement de
statut des écarts traités.

## 4.2.7Revue et rétrospective du Sprint 5

La revue confirme que les écarts deviennent exploitables. La rétrospective souligne la nécessité de
garder la logique de comparaison côté serveur.

## 4.3Sprint 6 : clôture, consolidation et export

## 4.3.1Objectifs du Sprint 6

Le Sprint 6 vise à consolider les résultats d’inventaire et à préparer l’export final exploitable par les
équipes métier.

## 4.3.2Backlog du Sprint 6

–Consolider les résultats après C1 ou C2.

–Préparer la clôture de l’inventaire.

–Générer un fichier final exploitable.

–Prévoir l’extension Branche B.

---

## 4.3.3Planification des tâches du Sprint 6

Tableau 4.5:Planification des tâches du Sprint 6

| Tâche | Description | Livrable |
| --- | --- | --- |
| Consolidation | Fusionner théorique, scans et écarts | Service finalisation |
| Clôture | Définir les conditions de fin | Statut inventaire |
| Export | Préparer le fichier de sortie | Export Excel |
| Extension | Prévoir le recomptage global Branche B | Points d’évolution |

## 4.3.4Conception du Sprint 6

La conception formalise les états de clôture et les règles de consolidation. Elle distingue le recomptage
ciblé déjà couvert par la Branche A et l’extension Branche B.

## 4.3.5Réalisation du Sprint 6

La réalisation met en place les services de consolidation, les premières fonctions d’export et les points
d’extension nécessaires à la finalisation globale.

## 4.3.6Tests du Sprint 6

Les tests vérifient la consolidation après C1, après C2, l’export des résultats et la cohérence des statuts
d’inventaire.

## 4.3.7Revue et rétrospective du Sprint 6

La revue valide le principe de clôture. La rétrospective identifie l’export final comme un point à renforcer
lors des itérations de durcissement.

## Conclusion

La Release 2 rend le comptage résilient et transforme les données terrain en résultats structurés. Les
scans peuvent être conservés puis synchronisés sans doublon, les écarts sont détectés et un passage C2
ciblé peut être créé. La consolidation prépare enfin l’exploitation décisionnelle et la mise en service
traitées dans la release suivante.

---

Release 3 : pilotage, validation et

## mise en service

---

## Plan

Introduction . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 51

1 Sprint 7 : dashboard KPI et analytics . . . . . . . . . . . . . . . 51

2 Sprint 8 : tests, documentation et validation . . . . . . . . . . 52

3 Sprint 9 : déploiement local . . . . . . . . . . . . . . . . . . . . 53

Conclusion . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 54

---

## Introduction

La troisième release finalise la valeur opérationnelle de la plateforme. Elle transforme les données
consolidées en indicateurs de pilotage, stabilise les parcours critiques par des scénarios de validation
et prépare l’utilisation de la solution sur le réseau local de VECTORYS. Cette release regroupe les
Sprints 7, 8 et 9.

## 5.1Sprint 7 : dashboard KPI et analytics

## 5.1.1Objectifs du Sprint 7

Le Sprint 7 vise à transformer les données d’inventaire en indicateurs de pilotage pour le superviseur.

## 5.1.2Backlog du Sprint 7

–Calculer les KPI de précision, performance, qualité, client et WMS.

–Filtrer les indicateurs par inventaire, période et zone.

–Afficher les graphiques dans le portail Angular.

–Optimiser les requêtes par index SQL.

## 5.1.3Planification des tâches du Sprint 7

Tableau 5.1:Planification des tâches du Sprint 7

| Tâche | Description | Livrable |
| --- | --- | --- |
| Agrégats | Calculer les indicateurs depuis les scans et écarts | KpiRepository |
| API KPI | Exposer les endpoints de consultation | KpiController |
| Dashboard | Afficher les graphiques Chart.js | UI Angular |
| Optimisation | Ajouter les index utiles | Migrations SQL |

## 5.1.4Conception du Sprint 7

Les KPI sont conçus comme des agrégats calculés et non comme une entité métier persistée. Cette
approche évite les incohérences et garantit que les indicateurs reflètent les données à jour.

---

## 5.1.5Réalisation du Sprint 7

Le sprint livre les endpoints KPI, le repository d’agrégation, les filtres, les index SQL et le dashboard
Angular.

## 5.1.6Tests du Sprint 7

Les tests comparent les valeurs KPI calculées avec des jeux de données contrôlés et vérifient les filtres
par inventaire, période et zone.

## 5.1.7Revue et rétrospective du Sprint 7

La revue valide la lisibilité du tableau de bord. La rétrospective met en avant la nécessité d’optimiser
les requêtes avant l’exploitation sur des volumes réels.

## 5.2Sprint 8 : tests, documentation et validation

## 5.2.1Objectifs du Sprint 8

Le Sprint 8 vise à sécuriser la qualité fonctionnelle avant la mise en service locale.

## 5.2.2Backlog du Sprint 8

–Documenter les endpoints avec Swagger.

–Préparer les scénarios Postman.

–Construire les jeux de données de test.

–Valider les parcours critiques de bout en bout.

## 5.2.3Planification des tâches du Sprint 8

Tableau 5.3:Planification des tâches du Sprint 8

| Tâche | Description | Livrable |
| --- | --- | --- |
| Swagger | Documenter l&#x27;API REST | Documentation API |
| Scénarios | Définir les parcours de validation | Cas de test |
| Jeux de données | Préparer inventaires, agents et stocks | Données de test |
| Corrections | Résoudre les anomalies bloquantes | Version stabilisée |

---

## 5.2.4Conception du Sprint 8

La conception des tests couvre les parcours d’authentification, d’import, de scan, de fonctionnement
hors ligne, de traitement des écarts et de consultation des KPI. Chaque scénario précise ses
préconditions, ses étapes, le résultat attendu et le critère de succès.

## 5.2.5Réalisation du Sprint 8

La réalisation produit la documentation Swagger, les scénarios de validation, les jeux de test et les
corrections issues des essais.

## 5.2.6Tests du Sprint 8

Les tests sont au coeur du sprint : ils valident les endpoints API, les interfaces web, les écrans mobiles
et les workflows de bout en bout.

## 5.2.7Revue et rétrospective du Sprint 8

La revue confirme que le périmètre prioritaire est validé. La rétrospective distingue les anomalies
bloquantes corrigées des améliorations reportées.

## 5.3Sprint 9 : déploiement local

## 5.3.1Objectifs du Sprint 9

Le Sprint 9 prépare l’exploitation interne de la solution sur un serveur local du réseau VECTORYS.

## 5.3.2Backlog du Sprint 9

–Publier l’API ASP.NET Core sur serveur local Windows.

–Servir le portail Angular sur le réseau local.

–Générer l’APK Flutter pointant vers l’adresse IP du serveur.

–Configurer CORS, SignalR, chaîne SQL Server et pare-feu.

## 5.3.3Planification des tâches du Sprint 9

Tableau 5.5:Planification des tâches du Sprint 9

| Tâche | Description | Livrable |
| --- | --- | --- |
| Publication API | Générer le build backend | API publiée |

---

Chapitre 5. Release 3 : pilotage, validation et mise en service

| Tâche | Description | Livrable |
| --- | --- | --- |
| Portail web | Servir les fichiers Angular | Interface LAN |
| APK mobile | Configurer l&#x27;URL serveur | APK terrain |
| Configuration réseau | Ouvrir ports, CORS et SignalR | Déploiement local |

## 5.3.4Conception du Sprint 9

La conception cible une topologie locale : API, SQL Server et portail web sur le réseau VECTORYS,
avec des terminaux mobiles connectés à l’adresse IP privée du serveur.

## 5.3.5Réalisation du Sprint 9

Le sprint livre les publications applicatives, la configuration serveur, l’APK de démonstration et la
validation de bout en bout sur le réseau local.

## 5.3.6Tests du Sprint 9

Les tests vérifient l’accès au portail depuis un poste superviseur, la connexion mobile à l’API locale,
la synchronisation SignalR et l’accès à SQL Server.

## 5.3.7Revue et rétrospective du Sprint 9

La revue valide la mise en service locale. La rétrospective confirme que le choix d’un serveur local est
cohérent avec le contexte interne et les contraintes réseau de VECTORYS.

## Conclusion

La Release 3 clôture le cycle de livraison. Elle fournit au superviseur les indicateurs nécessaires au
pilotage, valide les principaux parcours fonctionnels et prépare l’exécution de l’API, du portail et de
l’application mobile sur le réseau local de VECTORYS. Les trois releases couvrent ainsi progressivement
la préparation, l’exécution, la résilience, l’analyse et la mise en service de l’inventaire.

---

## Conclusion générale

Au terme de ce projet de fin d’études, la problématique posée au chapitre 1 — concevoir une plateforme
d’inventaire multi-canal fiable, capable de synchroniser le comptage terrain et le pilotage superviseur,
y compris hors connexion — a trouvé une réponse concrète à travers*Vectorys Inventaire*.

Le cheminement du rapport a permis de passer d’une analyse métier (acteurs, RF/RNF, cas
d’utilisation, backlog Scrum) à une conception technique structurée (architecture three-tier, modèle
de classes, séquences par fonctionnalité), puis à une réalisation multi-canal (API Clean Architecture,
portail Angular, application Flutter offline) et enfin à une validation par scénarios reproductibles.
La plateforme couvre désormais le cœur du cycle inventaire VECTORYS : import Excel, assignation
CodeAgent, scan à quotas, auto-terminaison, synchronisation batch, détection d’écarts avec branche
A, suivi temps réel et indicateurs KPI.

Sur le plan méthodologique, Scrum a favorisé des livraisons incrémentales et une priorisation guidée
par la valeur terrain. Sur le plan organisationnel, le binôme encadrant professionnel / encadrante
académique a ancré les choix techniques dans les contraintes réelles de l’entrepôt : zones couvertes
partiellement, fichiers ERP hétérogènes et besoin de supervision immédiate.

Les limites identifiées — couverture encore partielle des tests unitaires automatisés, finalisation de
certains workflows de clôture/export, captures de déploiement LAN encore à compléter — n’effacent
pas le résultat principal : une plateforme opérationnelle, documentée et prête à être mise en service
sur un serveur local du réseau VECTORYS.

Ce travail a également constitué une expérience professionnelle complète : compréhension d’un
métier logistique 3PL, conception logicielle en Clean Architecture, développement multi-technologie
et conduite de tests d’acceptation. Il ouvre naturellement sur les perspectives exposées ci-après.

## Perspectives

Plusieurs axes d’évolution se dégagent à l’issue du stage :

1.Mise en service sur serveur local.Publier l’API et le portail sur la machine d’entreprise,
configurer CORS/SignalR et générer l’APK avec API_BASE_URL pointant vers l’IP du
serveur; compléter alors les figures de déploiement local.

2.Consolidation des workflows de fin d’inventaire.Formaliser la clôture inventaire (statut
CLOTURE), l’export Excel des résultats et, selon besoin terrain, le recomptage global (branche
B).

---

3.Automatisation des tests.Enrichir la suite xUnit / Flutter Test autour des handlers critiques
(quotas, écarts, import) pour sécuriser les régressions.

4.Enrichissement analytique.Affiner les KPI WMS/clients (référentiel client, valeur monétaire
des écarts, occupation maximale) à partir de données ERP complémentarisées.

5.Expérience terrain.Intégrer les retours opérateurs (ergonomie scan, alertes, impressions de
rapports) et éventuellement un mode tablette pour les chefs d’équipe.

6.Industrialisation.Prévoir scripts de publication, secrets hors dépôt, journaling opérationnel
et procédure de reprise après incident.

Ces perspectives marquent la transition entre un livrable académique réussi et un produit d’exploitation
durable au sein de VECTORYS Tunisie.

---

## Bibliographie

[B1] Robert C. Martin,*Clean Architecture : A Craftsman’s Guide to Software Structure and Design*,
Prentice Hall, 2017.
[B2] Eric Evans,*Domain-Driven Design : Tackling Complexity in the Heart of Software*,
Addison-Wesley, 2003.
[B3] Martin Fowler,*Patterns of Enterprise Application Architecture*, Addison-Wesley, 2002.
[B4] Craig Larman,*UML 2 et les design patterns : analyse et conception orientées objet*, Pearson
Education, 2005.
[B5] Ken Schwaber et Jeff Sutherland,*The Scrum Guide : The Definitive Guide to Scrum*, Scrum.org,
2020.
[B6] Sam Newman,*Building Microservices : Designing Fine-Grained Systems*, O’Reilly Media, 2021.
[B7] Len Bass, Paul Clements et Rick Kazman,*Software Architecture in Practice*, Addison-Wesley,
2021.
[B8] Microsoft,*Architecting Modern Web Applications with ASP.NET Core and Azure*, Microsoft Press
/ Microsoft Learn, 2024.

---

## Webographie

[1]« VECTORYS — Site officiel. » [Consulté le 13/07/2026]. adresse :https://www.vectorys.com/
[2]« TEK-UP University — Site officiel. » [Consulté le 13/07/2026]. adresse :https://tek-up.de/
[3]K.Schwaberet J.Sutherland. « The Scrum Guide. » [Consulté le 13/07/2026]. adresse :
https://scrumguides.org/scrum-guide.html
[4]Angular Team. « Angular Documentation. » [Consulté le 13/07/2026]. adresse :https://
angular.dev/
[5]Flutter Team. « Flutter Documentation. » [Consulté le 13/07/2026]. adresse :https://docs.
flutter.dev/
[6]Dart Team. « Dart Documentation. » [Consulté le 13/07/2026]. adresse :https://dart.dev/
guides
[7]Microsoft. « .NET Documentation. » [Consulté le 13/07/2026]. adresse :https : / / learn .
microsoft.com/dotnet/
[8]Microsoft. « ASP.NET Core Documentation. » [Consulté le 13/07/2026]. adresse :https :
//learn.microsoft.com/aspnet/core/
[9]Microsoft. « Entity Framework Core Documentation. » [Consulté le 13/07/2026]. adresse :
https://learn.microsoft.com/ef/core/
[10]Microsoft. « SQL Server Documentation. » [Consulté le 13/07/2026]. adresse :https : / /
learn.microsoft.com/sql/sql-server/
[11]Microsoft. « ASP.NET Core SignalR Documentation. » [Consulté le 13/07/2026]. adresse :
https://learn.microsoft.com/aspnet/core/signalr/introduction
[12]M.Jones, J.Bradleyet N.Sakimura. « RFC 7519 — JSON Web Token (JWT). » [Consulté
le 13/07/2026]. adresse :https://www.rfc-editor.org/rfc/rfc7519
[13]OWASP Foundation. « Password Storage Cheat Sheet. » [Consulté le 13/07/2026]. adresse :
https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
[14]J.Bogard. « MediatR — Simple mediator implementation in .NET. » [Consulté le 13/07/2026].
adresse :https://github.com/jbogard/MediatR
[15]FluentValidation. « FluentValidation Documentation. » [Consulté le 13/07/2026]. adresse :
https://docs.fluentvalidation.net/

---

[16]ClosedXML. « ClosedXML Documentation. » [Consulté le 13/07/2026]. adresse :https://
docs.closedxml.io/
[17]SQLite Consortium. « SQLite Documentation. » [Consulté le 13/07/2026]. adresse :https:
//www.sqlite.org/docs.html
[18]OpenAPI Initiative. « OpenAPI Specification. » [Consulté le 13/07/2026]. adresse :https:
//spec.openapis.org/oas/latest.html
[19]Chart.js. « Chart.js Documentation. » [Consulté le 13/07/2026]. adresse :https : / / www .
chartjs.org/docs/latest/
[20]Microsoft. « Architect modern web applications with ASP.NET Core and Azure. » [Consulté
le 13/07/2026]. adresse :https://learn.microsoft.com/dotnet/architecture/modern-webapps-azure/
[21]Microsoft. « Warehouse management overview. » [Consulté le 13/07/2026]. adresse :https:
//learn.microsoft.com/dynamics365/supply-chain/warehousing/warehouse-managementoverview
[22]OpenAPI Initiative. « OpenAPI Specification — Swagger. » [Consulté le 13/07/2026].
adresse :https://swagger.io/specification/
[23]Object Management Group. « Unified Modeling Language (UML) Specification. » [Consulté
le 13/07/2026]. adresse :https://www.omg.org/spec/UML/
[24]VECTORYS Tunisie,*Documentation interne et besoins métier inventaire*, Notes de stage et
entretiens avec l’encadrant professionnel, 2025–2026.
[25]TEK-UP,*Template et consignes de rédaction du rapport de Projet de Fin d’Études*, École
Supérieure Privée d’Ingénierie et de Technologie, 2025–2026.

---

## Résumé

Ce projet de fin d’études, réalisé au sein de VECTORYS Tunisie, porte sur la conception et le développement
de*Vectorys Inventaire*, une plateforme d’inventaire logistique multi-canal. L’objectif est de digitaliser le cycle
complet de l’inventaire physique en entrepôt : préparation (création de mission, import du stock théorique depuis
Excel, assignation stricte des agents), exécution terrain (scan des unités de stock, quotas, clôture automatique des
emplacements, mode hors ligne), traitement des écarts (détection MANQUANT/SURPLUS, recomptage ciblé)
et pilotage (suivi temps réel SignalR, tableaux de bord KPI). Sur le plan technique, la solution repose sur une
architecture three-tier : une API REST ASP.NET Core 9 (Clean Architecture, CQRS/MediatR, FluentValidation,
JWT, Entity Framework Core, ClosedXML), une application web Angular 19 destinée au superviseur, et une
application mobile Flutter pour les agents, adossées à Microsoft SQL Server et à une base SQLite locale pour
la résilience hors connexion. Le projet a été mené selon la méthode agile Scrum, avec une phase préparatoire
suivie de neuf sprints répartis en trois releases. La validation a combiné tests d’intégration API (Swagger,
Postman), scénarios bout-en-bout import/comptage et contrôles en base de données. Le déploiement retenu pour
l’exploitation interne est un serveur local sur le réseau d’entreprise.

Mots clés :inventaire logistique, multi-canal, ASP.NET Core, Angular, Flutter, SQL Server, SignalR, Scrum,
KPI, entrepôt, hors ligne

## Abstract

This graduation project, carried out at VECTORYS Tunisia, focuses on the design and development of
*Vectorys Inventaire*, a multi-channel logistics inventory platform. The goal is to digitize the full physical
warehouse inventory cycle: preparation (mission creation, Excel import of theoretical stock, strict agent
assignment), field execution (unit scanning, quotas, automatic location closure, offline mode), discrepancy
handling (shortage/surplus detection, targeted recount), and supervision (real-time SignalR monitoring, KPI
dashboards). Technically, the solution follows a three-tier architecture: an ASP.NET Core 9 REST API (Clean
Architecture, CQRS/MediatR, FluentValidation, JWT, Entity Framework Core, ClosedXML), an Angular 19
web portal for supervisors, and a Flutter mobile application for agents, backed by Microsoft SQL Server and
a local SQLite store for offline resilience. The work was organized with Scrum through a preliminary setup
phase followed by nine sprints divided into three releases. Validation combined API integration tests (Swagger,
Postman), end-to-end import/counting scenarios, and database checks. The deployment model chosen for internal
use is a local on-premises server on the corporate LAN.

Keywords :logistics inventory, multi-channel, ASP.NET Core, Angular, Flutter, SQL Server, SignalR, Scrum,
KPI, warehouse, offline