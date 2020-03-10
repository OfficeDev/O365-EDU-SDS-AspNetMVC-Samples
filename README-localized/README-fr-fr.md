---
page_type: sample
products:
- office-365
- ms-graph
languages:
- csharp
- aspx
extensions:
  contentType: samples
  technologies:
  - Microsoft Graph
  services:
  - Office 365
  - Education
  createdDate: 9/20/2017 5:37:15 PM
  scenarios:
  - Eduation
description: "Ce projet implémente les comportements OneRoster v1.1."
---

# Exemple de fournisseur OneRoster REST

Ce projet implémente les comportements OneRoster v1.1.

Il cible spécifiquement les conditions requises pour :

1. Création de listes de présence OneRoster v 1.1 fournisseur REST
2. Carnet de notes OneRoster v 1.1 Fournisseur REST

**Table des matières**
* [Exemples de objectifs](#sample-goals)
* [Conditions préalables](#prerequisites)
* [Créer et déboguer localement](#build-and-debug-locally)
* [Déploiement de l’exemple vers Azure](#deploy-the-sample-to-azure)
* [Synchroniser avec SDS](#sync-with-sds)
* [Comprendre le code](#understand-the-code)
* [Points de terminaison OneRoster pris en charge](#supported-oneroster-endpoints)
* [Fonctionnalités non prises en charge](#unsupported-features)
* [Questions et commentaires](#questions-and-comments) [Contribution](#contributing)

## Exemples de objectifs

Cet exemple illustre les éléments suivants :

* Modèles de données prenant en charge les [entités de OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006)
* Contrôleurs d’action prenant en charge les [points de terminaison de OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989)
* Intergiciel d'autorisation prenant en charge [Sécurité de base de OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452001)
* Génération de [Lots CSV OneRoster](https://www.imsglobal.org/oneroster-v11-final-csv-tables)
* Consommation des [API de gestion de profil SDS](https://github.com/OfficeDev/O365-EDU-Tools/tree/master/SDSProfileManagementDocs) pour créer et démarrer des profils de synchronisation SDS

OneRosterProviderDemo est basé sur ASP.NET Core Web.

## Conditions préalables

**Déploiement et l’exécution de cet exemple nécessitent** :

* Un abonnement Azure disposant des autorisations pour inscrire une nouvelle application et déployer l’application web.
* Un client Office 365 Éducation avec Microsoft School Data Sync activé
* Un des navigateurs suivants : Edge, Internet Explorer 9, Safari 5.0.6, Firefox 5, Chrome 13 ou une version ultérieure de l’un de ces navigateurs.
* Outil permettant de générer des signatures OAuth1, telles que [Postman](https://www.getpostman.com/)
* Visual Studio 2017 (toute édition), [Communauté Visual Studio 2017](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community) est disponible gratuitement.
* Familiarisez-vous avec C# , applications web .Net et les services web.

## Inscription de l’application dans Azure Active Directory

1. Connectez-vous au nouveau Portail Azure : [https://portal.azure.com](https://portal.azure.com/).

2. Cliquez sur **Azure Active Directory**, -> **Inscriptions des applications** -> **+Ajouter**.

   ![](Images/aad-create-app-01.png)

3. Entrez un **Nom**, puis sélectionnez **Application web/API** en tant que**Type d’application**.

   **URL de connexion**: https://localhost :44344/

   ![](Images/aad-create-app-02.png)

   Cliquez sur **Créer**.

4. Une fois l’opération terminée, l’application s’affiche dans la liste.

   ![](/Images/aad-create-app-03.png)

5. Cliquez dessus pour afficher les détails associés.

   ![](/Images/aad-create-app-04.png)

6. Cliquez sur **Tous les paramètres**, si la fenêtre de paramètres ne s’affiche pas.

   * Cliquez sur **Propriétés**, puis configurez **Mutualisé** à **Oui**.

     Copiez **ID d’application**, puis cliquez **Enregistrer**.

   * Cliquez sur **Autorisations requises**. Ajoutez les autorisations suivantes :

     | API | Autorisations déléguées |
| ------------------------------ | ---------------------------------------- |
| Microsoft Graph | Lire des données d’annuaire<br>Lire et écrire les données de l’annuaire<br>Accéder à l’annuaire en tant qu’utilisateur connecté<br>Lire les paramètres d’application d’enseignement<br>Gérer les paramètres de l’application éducation |
| Windows Azure Active Directory |
| Se connecter et lire le profil de l’utilisateur |

   * Cliquez sur **Clés**, puis ajoutez une nouvelle clé :

     ![](Images/aad-create-app-07.png)

     Cliquez sur **Enregistrer**, puis copiez la **VALEUR** de la clé.

   Fermez la fenêtre Paramètres.

## Créez et déboguez localement

Vous pouvez ouvrir ce projet avec l’édition de Visual Studio 2017 que vous avez déjà, ou télécharger et installer l'édition de la Communauté pour exécuter, construire et/ou développer cette application localement.

- [Communauté Visual Studio 2017](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community)

Déboguez **OneRosterProviderDemo** :

1. Configurez **appsettings.json**.

   ![](Images/web-app-config.png)

   - **AzureAd:Authority** : "https://login.microsoftonline.com/{your-ad-tenant}.onmicrosoft.com/"
   - **AzureAd:ClientId** : utilisez l’ID client de l’inscription de l’application que vous avez créée précédemment.
   - **AzureAd:ClientSecret**: utilisez la valeur clé de l’inscription de l’application que vous avez créée précédemment.
   - **AzureDomain** : utilisez votre domaine de locataire Active Directory, lequel doit accepter l’entrée **AzureAd:Authority**

2. Dans la console du gestionnaire de package, exécutez la commande `EntityFrameworkCore\Update-Database` pour générer la base de données initiale. Si cela génère une erreur, essayez d’exécuter la commande `Import-Package Microsoft.EntityFrameworkCore`.
3. Sélectionnez **OneRosterProviderDemo** comme projet de démarrage, puis appuyez sur le touche F5.
4. Visitez `/seeds` pour remplir votre base de données avec des exemples d’entités. Veillez à accéder au point de terminaison à l’aide de HTTPs.

## Déployez l’exemple sur Azure

1. Créer un profil de publication

  - Sélectionnez **Build > Publier OneRosterProviderDemo**
  - Cliquez sur **Créer un nouveau profil**.
  - Sélectionnez **Créer un Microsoft Azure App Service** et **Créer nouveau**
  - Cliquez sur **Publier**.
  - Connectez-vous à votre compte Azure
  - Sélectionnez un groupe de ressources pour le déploiement ou créez-en un en cliquant sur **Nouveau**
  - Sélectionnez un plan de service d’application pour le déploiement ou créez-en un en cliquant sur **Nouveau**
  - Cliquez sur **Créer**

2. Sélectionnez **Build > Publier OneRosterProviderDemo**
3. Cliquez sur **Publier**.

**Ajouter une URL de réponse à l’inscription de l’application**

1. Après le déploiement, ouvrez le groupe de ressources dans le Portail Azure
2. Cliquez sur l’application web.

   ![](Images/azure-web-app.png)

   Copiez l’URL et modifiez le schéma en **https** et ajoutez **/signin-oidc** à la fin. Il s’agit de l’URL de réponse qui sera utilisée à l’étape suivante.

3. Accédez à inscription de l’application dans le nouveau Portail Azure, puis ouvrez le paramètre Windows.

   Ajouter l’URL de réponse :

   ![](Images/aad-add-reply-url.png)

   > Remarque : pour déboguer l’exemple localement, assurez-vous que https://localhost:44344/signin-oidc se trouve dans les URL de réponse.

4. Cliquez sur **ENREGISTRER**.

## Synchronisation avec SDS

Il existe deux options de synchronisation, via les points de terminaison OneRoster REST et le téléchargement [Fichiers CSV conformes à la norme SDS](https://support.office.com/en-us/article/CSV-files-for-School-Data-Sync-9f3c3c2b-7364-4f6e-a959-e8538feead70).

### Synchronisation avec OneRoster REST
1. Visitez votre déploiement sur /sds
2. Connectez-vous à l’aide d’un compte d’administrateur sur votre locataire éducation.
3. Vérifiez que la valeur « point de terminaison OneRoster » accepte avec votre instance déployée
  - Veuillez noter que cela ne fonctionnera que si votre URL est accessible au public.
4. Cliquez sur le bouton « Synchroniser via OneRoster REST »

### Synchronisation avec CSV de SDS
1. Visitez votre déploiement sur /sds
2. Connectez-vous à l’aide d’un compte d’administrateur sur votre locataire éducation.
3. Sélectionnez vos fichiers CSV de SDS
4. Cliquez sur le bouton « Synchroniser via CSV de SDS »

## Comprendre le code

### Introduction

Cette application web est basée sur un modèle de projet web ASP.NET Core.

### Autorisation

#### Points de terminaison OneRoster
Le fichier `Middlewares/OAuth.cs` définit un intergiciel qui valide la signature OAuth1 ou Oauth2 ne pour chaque demande entrante avec un itinéraire de OneRoster. Ce fichier contient également l’ID de client codé en dur et le code secret. Pour que les demandes Oauth2 ne soient émises, visitez tout d’abord le point de terminaison /token à l’aide des informations d’identification OAuth1 pour obtenir le jeton d’accès. Le fichier `Startup.cs` configure l’application de façon à ce qu’elle utilise cet intergiciel.

#### Gestion des profils SDS
Le fichier `Startup.cs` configure l’application de façon à utiliser la bibliothèque OpenIDConnect (oidc) de .NET Core. Ce flux est maintenant géré par `AccountController`.

### Modèles de données

La plupart des modèles OneRoster ont une classe de modèle correspondante dans l’annuaire `Modèles`. En raison des conventions d’affectation des noms de fichiers de langue, certaines ont été renommées. Le mappage des modèles est indiquée dans le tableau ci-dessous.

| Modèle OneRoster | Modèle EFCore |
|------------------------|-------------------|
| Base | BaseModel |
| Session éducation | AcademicSession |
| Classe | IMSClass |
| Cours | Cours |
| Données démographiques | Démographiques |
| Inscription | Inscription |
| Élément de ligne | LineItem |
| Catégorie de ligne | LineItemCategory |
| Organisation | Organisation |
| Ressource | Ressource |
| Résultat | Résultat |
| Utilisateur, étudiant, enseignant | Utilisateur |

Des modèles supplémentaires ont été créés afin de répondre aux besoins variés :

| Modèle EFCore | Objectif |
|-------------------------|-------------------------------------------------|
| IMSClassAcademicSession | Table de réunion pour la classe, le trimestre (session éducation) |
| UserAgent | Table de réunion pour Utilisateur, Utilisateur |
| UserOrg | Table de réunion pour Utilisateur, Organisation |
| UserId | Forme des ID utilisateurs |
| SeedData | Contient des données de base pour la population initiale de la base de données |
| OauthNonce | Stocke les valeurs nonce pour interdire la réutilisation |

### Validation

Les conditions requises en matière de validation sont définies dans la [Spécification de modèle OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006).

Outre les attributs de validation fournis par .NET et ASP Core, il existe quatre validateurs personnalisés pour les propriétés qui ont des exigences spécifiques dans le dossier `Programmes de validation`.

#### Vocabulaire
les [Vocabulaires ](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452048) utilisés par OneRoster, y compris les [Énumérations](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452020), sont fournis par le cours `Vocabulaire`.

Le vocabulaire de code de sujet « SCEDv4.0 » est publié par [NCES](https://nces.ed.gov/forum/SCED.asp); une conversion de la feuille de calcul publiée est présente dans le fichier `Vocabulary/sced-v4.csv`, qui est analysé au démarrage.

### Sérialisation personnalisée

Tous les objets json de réponse sont écrits à l’aide du package nuget json de NewtonSoft, dans le but de faire correspondre les [liaisons json prescrites](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452033).

Toutes les entités pouvant être sérialisées en une réponse de point de terminaison de service prennent en charge deux méthodes de sérialisation, `AsJson` et `AsJsonReference`.

**AsJson** est utilisée lorsque l’entité est l’entité principale ou un membre de la collection principale demandée.

**AsJsonReference** est utilisé pour exprimer l’entité sous la forme d’un[GUIDRef](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452031).

### Pagination, filtrage et tri

Le cours `Controllers/BaseController` s'occupe de la [pagination, filtrage, et tri](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451994).

Le filtrage et le tri sont implémentés dans les cours de modèle via la réflexion afin de prendre en charge la nécessité qu’un champ de données soit utilisable pour le filtrage ou le tri.

### Source de données

Une base de données SQLite est supposée et configurée dans `Startup.cs`.

Vous pouvez créer une nouvelle base de données vide en exécutant la commande `EntityFrameworkCore\Update-Database` dans la console du gestionnaire de package, et l’amorcer avec des exemples de données en visitant le point de terminaison `/seeds`.

## Points de terminaison OneRoster pris en charge

Trois sous-ensembles de service OneRoster v1.1 sont définis [ici](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989). Les sous-ensembles de la liste (lecture seule) et carnet de notes (lecture et écriture) sont implémentés par cet exemple.

### Points de terminaison de création de listes de présence (Lecture)

| Appel de service | Point de terminaison | Verbe HTTP |
|--------------------------------|-----------------------------------------------------|-----------|
| getAllAcademicSessions | /academicSessions | OBTENIR |
| getAcademicSession | /academicSessions/{id} | OBTENIR |
| getAllClasses | /classes | OBTENIR |
| getClass | /classes/{ID} | OBTENIR |
| getStudentsForClass | /classes/{class\_id}/Students | OBTENIR |
| getTeachersForClass | /classes/{class\_id}/Teachers | OBTENIR |
| getAllCourses | /courses | OBTENIR |
| getCourse | /courses/{ID} | OBTENIR |
| getClassesForCourse | /courses/{course\_id}/classes | OBTENIR |
| getAllDemographics | /Demographics | OBTENIR |
| getDemographics | /Demographics/{ID} | OBTENIR |
| getAllEnrollments | /Enrollments | OBTENIR |
| getEnrollment | /Enrollments/{ID} | OBTENIR |
| getAllGradingPeriods | /gradingPeriods | OBTENIR |
| getGradingPeriod | /gradingPeriods/{id} | OBTENIR |
| getAllOrgs | /orgs | OBTENIR |
| getOrg | /orgs/{ID} | OBTENIR |
| getAllSchools | /Schools | OBTENIR |
| getSchool | /Schools/{ID} | OBTENIR |
| getCoursesForSchool | /Schools/{ID}/courses | OBTENIR |
| getClassesForSchool | /Schools/{school\_id}/classes | OBTENIR |
| getEnrollmentsForSchool | /Schools/{school\_id}/Enrollments | OBTENIR |
| getEnrollmentsForClassInSchool | /Schools/{school\_id}/classes/class\_id}/Enrollments | OBTENIR |
| getStudentsForClassInSchool | /Schools/{school\_id}/classes/{class\_id}/Students | OBTENIR |
| getStudentsForSchool | /Schools/{school\_id}/Students | OBTENIR |
| getTeachersForClassInSchool | /Schools/{school\_id}/classes/{class\_id}/Teachers | OBTENIR |
| getTeachersForSchool | /Schools/{school\_id}/Teachers | OBTENIR |
| getTermsForSchool | /Schools/{school\_id}/Terms | OBTENIR |
| getAllStudents | /Students | OBTENIR |
| getStudent | /Students/{ID} | OBTENIR |
| getClassesForStudent | /Students/{ID}/classes | OBTENIR |
| getAllTeachers | /Teachers | OBTENIR |
| getTeacher | /Teachers/{ID} | OBTENIR |
| getClassesForTeacher | /Teachers/{ID}/classes | OBTENIR |
| getAllTerms | /Terms | OBTENIR |
| getTer | /Terms/{ID} | OBTENIR |
| getClassesForTerm | /Terms/{ID}/classes | OBTENIR |
| getGradingPeriodsForTerm | /terms/{id}/gradingPeriods | OBTENIR |
| getAllUsers | /Users | OBTENIR |
| getUser | /Users/{ID} | OBTENIR |
| getClassesForUser | /Users/{ID}/classes | OBTENIR |


### Points de terminaison du carnet de notes (Lecture)

| Appel de service | Point de terminaison | Verbe HTTP |
|-------------------------------|---------------------------------------------------|-----------|
| getAllCategories | /Categories | OBTENIR |
| getCategory | /Categories/{ID} | OBTENIR |
| getAllLineItems | /lineItems | OBTENIR |
| getLineItem | /lineItems/{id} | OBTENIR |
| getAllResults | /Results | OBTENIR |
| getResult | /Results/{ID} | OBTENIR |
| getResultsForClass | /classes/{class\_id}/Results | OBTENIR |
| getLineItemsForClass | /classes/{class\_id}/lineItems | OBTENIR |
| getResultsForLineItemForClass | /classes/{class\_id}/lineItems/{li\_id}/Results | OBTENIR |
| getResultsForStudentForClass | /classes/{class\_id}/Students/{student\_id}/Results | OBTENIR |

### Points de terminaison du carnet de notes (Écriture)

| Appel de service | Point de terminaison | Verbe HTTP |
|-------------------------------|---------------------------------------------------|-----------|
| deleteCategory | /Categories/{ID} | SUPPRIMER |
| putCategory | /Categories/{ID} | PUT |
| deleteLineItem | /lineItems/{id} | SUPPRIMER |
| putLineItem | /lineItems/{id} | PUT |
| deleteResult | /Results/{ID} | SUPPRIMER |
| putResult | /Results/{ID} | PUT |

## Fonctionnalités non prises en charge

Pour l’instant, les comportements suivants ne sont pas inclus dans la spécification :

* [Filtrage des propriétés imbriquées](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451997)
* [Selection du champ](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451998)

## Questions et commentaires

* Si vous rencontrez des difficultés pour exécuter cet exemple, veuillez [consigner un problème](https://github.com/OfficeDev/O365-EDU-SDS-AspNetMVC-Samples/issues).
* Si vous avez des questions sur le développement ASP Core en général, envoyez-les sur [Stack Overflow](https://stackoverflow.com/questions/tagged/asp.net-core). Assurez-vous que vos questions ou commentaires portent les mentions \[asp.net-core].
* Si vous avez des questions sur le développement OneRoster en général, envoyez-les sur [IMS Global](https://www.imsglobal.org/forums/ims-glc-public-forums-and-resources/oneroster-public-forum).

## Contribution

Ce projet autorise les contributions et les suggestions.
Pour la plupart des contributions, vous devez accepter le contrat de licence de contributeur (CLA, Contributor License Agreement) stipulant que vous êtes en mesure, et que vous vous y engagez, de nous accorder les droits d’utiliser votre contribution.
Pour plus d’informations, visitez https://cla.microsoft.com.

Lorsque vous soumettez une requête de tirage, un robot CLA détermine automatiquement si vous devez fournir un CLA et si vous devez remplir la requête de tirage appropriée (par exemple, étiquette, commentaire).
Suivez simplement les instructions données par le robot.
Vous ne devrez le faire qu’une seule fois au sein de tous les référentiels à l’aide du CLA.

Ce projet a adopté le [code de conduite Open Source de Microsoft](https://opensource.microsoft.com/codeofconduct/).
Pour en savoir plus, reportez-vous à la [FAQ relative au code de conduite](https://opensource.microsoft.com/codeofconduct/faq/)
ou contactez [opencode@microsoft.com](mailto:opencode@microsoft.com) pour toute question ou tout commentaire.



**Copyright (c) 2018 Microsoft. Tous droits réservés.**
