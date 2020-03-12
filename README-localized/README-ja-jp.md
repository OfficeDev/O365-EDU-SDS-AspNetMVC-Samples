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
description: "このプロジェクトでは、OneRoster v 1.1 の動作を実装します。"
---

# OneRoster REST プロバイダーのサンプル

このプロジェクトでは、OneRoster v 1.1 の動作を実装します。

具体的には、次の要件を対象とします。

1. OneRoster v1.1 Rostering REST プロバイダー
2. OneRoster v1.1 Gradebook REST プロバイダー

**目次**
* [サンプルの目的](#sample-goals)
* [前提条件](#prerequisites)
* [ローカルでビルドしてデバッグする](#build-and-debug-locally)
* [Azure にサンプルを展開する](#deploy-the-sample-to-azure)
* [SDS との同期](#sync-with-sds)
* [コードを理解する](#understand-the-code)
* [サポートされている OneRoster エンドポイント](#supported-oneroster-endpoints)
* [サポート対象外の機能](#unsupported-features)
* [質問とコメント](#questions-and-comments)
* [投稿](#contributing)

## サンプルの目的

このサンプルでは、以下を例示します。

* [OneRoster エンティティ](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006)をサポートするデータ モデル
* [OneRoster サービス エンドポイント](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989)をサポートするアクション コントローラー
* [OneRoster Core Security](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452001) をサポートする承認ミドルウェア
* [OneRoster CSV バンドル](https://www.imsglobal.org/oneroster-v11-final-csv-tables)の生成
* SDS 同期プロファイルを作成して開始するための [SDS プロファイル管理 API](https://github.com/OfficeDev/O365-EDU-Tools/tree/master/SDSProfileManagementDocs) の消費

OneRosterProviderDemo は、ASP.NET Core Web に基づいています。

## 前提条件

**このサンプルを展開して実行するには、次のものが必要です**:

* 新しいアプリケーションを登録し、Web アプリを展開する権限を持つ Azure サブスクリプション。
* Microsoft School Data Sync が有効になっている O365 Education テナント
* 次のブラウザのいずれか:Microsoft Edge、Internet Explorer 9、Safari 5.0.6、Firefox 5、Chrome 13、これらのブラウザーのいずれかの最新バージョン。
* [Postman](https://www.getpostman.com/) など、OAuth1 署名を生成するためのツール。
* Visual Studio 2017 (すべてのエディション)、[Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community) は無料で提供されています。
* C#、.Net Web アプリケーション、および Web サービスに関する基礎知識。

## Azure Active Directory にアプリケーションを登録する

1. 新しい Azure ポータル ([https://portal.azure.com/](https://portal.azure.com/)) にサインインします。

2. [**Azure Active Directory**]、[**アプリの登録**]、[**+追加**] の順にクリックします。

   ![](Images/aad-create-app-01.png)

3. [**名前**] を入力し、[**アプリケーションの種類**] として [**Web アプリ/API**] を選択します。

   [**サインオン URL**] として 「https://localhost:44344/」と入力します。

   ![](Images/aad-create-app-02.png)

   [**作成**] をクリックします。

4. 完了すると、アプリが一覧に表示されます。

   ![](/Images/aad-create-app-03.png)

5. クリックして詳細を表示します。

   ![](/Images/aad-create-app-04.png)

6. [設定] ウィンドウが表示されない場合は、[**すべての設定**] をクリックします。

   * [**プロパティ**] をクリックし、[**マルチテナント**] を [**はい**] に設定します。

     [**アプリケーション ID**] を任意の場所にコピーし、[**保存**] をクリックします。

   * [**必要なアクセス許可**] をクリックします。以下のアクセス許可を追加します。

     | API | 委任されたアクセス許可 |
| ------------------------------ | ---------------------------------------- |
| Microsoft Graph | ディレクトリ データの読み取り<br>ディレクトリ データの読み取りと書き込み<br>サインインしたユーザーとしてディレクトリにアクセス<br>教育機関向けアプリの設定の読み取り<br>教育機関向けアプリの設定の管理 |
| Windows Azure Active Directory | サインインとユーザー プロファイルの読み取り |

   * [**キー**] をクリックし、新しいキーを追加します。

     ![](Images/aad-create-app-07.png)

     [**保存**] をクリックしてから、キーの**値**を任意の場所にコピーします。

   [設定] ウィンドウを閉じます。

## ローカルでビルドしてデバッグする

このアプリケーションをローカルで実行、ビルド、または開発するには、お客様が既に所有している Visual Studio 2017 のエディションでこのプロジェクトを開くことも、Community エディションをダウンロードしてインストールすることもできます。

- [Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community)

**OneRosterProviderDemo** をデバッグする

1. **appsettings.json** を構成します。

   ![](Images/web-app-config.png)

   - **AzureAd:Authority**: "https://login.microsoftonline.com/{your-ad-tenant}.onmicrosoft.com/"
   - **AzureAd:ClientId**: 先ほど作成したアプリ登録のクライアント ID を使用します。
   - **AzureAd:ClientSecret**: 先ほど作成したアプリ登録のキーの値を使用します。
   - **AzureDomain**: お客様の AD テナント ドメインを使用します。これは、**AzureAd:Authority** エントリと一致する必要があります。

2. パッケージ マネージャー コンソールでコマンド `EntityFrameworkCore\Update-Database` を実行し、初期データベースを生成します。このときにエラーが発生する場合は、コマンド `Import-Package Microsoft.EntityFrameworkCore` を実行してみてください。
3. StartUp プロジェクトとして **OneRosterProviderDemo** を設定し、F5 キーを押します。
4. `/seeds` にアクセスして、データベースにサンプル エンティティを追加します。必ず HTTPS を使用してエンドポイントにアクセスしてください。

## Azure にサンプルを展開する

1. 発行プロファイルを作成する

  - **[ビルド] > [OneRosterProviderDemo を発行]** の順に選択します。
  - [**新しいプロファイルの作成**] をクリックします。
  - [**Microsoft Azure App Service**] を選択し、[**新規作成**] を選択します。
  - [**発行**] をクリックします。
  - Azure アカウントにサインインします。
  - 展開用のリソースグ ループを選択するか、[**新規作成**] をクリックして新しく作成します。
  - 展開用の App Service プランを選択するか、[**新規作成**] をクリックして新しく作成します。
  - [**作成**] をクリックします。

2. **[ビルド] > [OneRosterProviderDemo を発行]** の順に選択します。
3. [**発行**] をクリックします。

**応答 URL をアプリの登録に追加する**

1. 展開が完了したら、Azure ポータルでリソース グループを開きます。
2. Web アプリをクリックします。

   ![](Images/azure-web-app.png)

   URL を任意の場所にコピーし、スキーマを「**https**」に変更し、末尾に 「**/signin-oidc**」 を追加します。これが応答 URL となり、次のステップで使用されます。

3. 新しい Azure ポータルでアプリの登録に移動し、[設定] ウィンドウを開きます。

   応答 URL を追加します。

   ![](Images/aad-add-reply-url.png)

   > 注: サンプルをローカルでデバッグするには、応答 URL として https://localhost:44344/signin-oidc が入力されていることを確認してください。

4. [**保存**] をクリックします。

## SDS との同期

同期には 2 つのオプションがあり、OneRoster REST エンドポイントを使用する方法と、[SDS に準拠する CSV ファイル](https://support.office.com/en-us/article/CSV-files-for-School-Data-Sync-9f3c3c2b-7364-4f6e-a959-e8538feead70)をアップロードする方法とがあります。

### OneRoster REST を使用して同期する
1. /sds で展開にアクセスします。
2. 教育機関のテナントで、管理者アカウントでサインインします。
3. "OneRoster endpoint" の値が展開されたインスタンスと一致していることを確認します。
  - 一般アクセスが可能な URL でない場合はこの方法を使用できません。
4. [OneRoster REST を使用して同期] ボタンをクリックします。

### SDS CSV を使用して同期する
1. /sds で展開にアクセスします。
2. 教育機関のテナントで、管理者アカウントでサインインします。
3. SDS CSV ファイルを選択します。
4. [SDS CSV を使用して同期] ボタンをクリックします。

## コードを理解する

### はじめに

この Web アプリケーションは、ASP.NET Core Web プロジェクト テンプレートに基づいています。

### 承認

#### OneRoster エンドポイント
`Middlewares/OAuth.cs` ファイルは、OneRoster ルートへのそれぞれの受信要求について、OAuth1 または OAuth2 署名を検証するミドルウェアを定義します。このファイルには、ハードコードされているクライアント ID とシークレットも含まれています。OAuth2 要求を行うには、まず OAuth1 資格情報を使用して /token エンドポイントにアクセスし、アクセス トークンを取得します。`Startup.cs` ファイルは、アプリがこのミドルウェアを使用するように構成します。

#### SDS プロファイル管理
`Startup.cs` ファイルは、アプリが .NET Core の OpenIDConnect (oidc) ライブラリを使うように構成します。このフローは、`AccountController` によって処理されます。

### データ モデル

OneRoster モデルのほとんどには、対応するモデル クラスが `Models` ディレクトリにあります。言語に関する命名規則のために、これらの一部では名前が変更されています。モデルのマッピングを次の表に示します。

| OneRoster モデル | EFCore モデル |
|------------------------|-------------------|
| Base | BaseModel |
| Academic Session | AcademicSession |
| Class | IMSClass |
| Course | Course |
| Demographic Data | Demographic |
| Enrollment | Enrollment |
| Line Item | LineItem |
| Line Item Category | LineItemCategory |
| Org | Org |
| Resource | Resource |
| Result | Result |
| User, Student, Teacher | User |

さまざまなニーズに対応するために追加のモデルが作成されました。

| EFCore モデル | 目的 |
|-------------------------|-------------------------------------------------|
| IMSClassAcademicSession | Join table for Class, Term (Academic Session) |
| UserAgent | Join table for User, User |
| UserOrg | Join table for User, Org |
| UserId | Shape of user ids |
| SeedData | Holds seed data for initial database population |
| OauthNonce | Stores nonce values to disallow reuse |

### 検証

検証要件は、[OneRoster モデルの仕様](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006)で定義されています。

.NET および ASP Core によって提供される検証属性に加えて、特定の要件を持つプロパティ用のカスタム データ検証ツールが `Validators` フォルダーにあります。

#### ボキャブラリ
[enumerations](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452020) を含め、OneRoster によって使用される[ボキャブラリ](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452048)は、`Vocabulary/Vocabulary` クラスによって提供されています。

サブジェクト コード ボキャブラリ "SCEDv 4.0" は [NCES](https://nces.ed.gov/forum/SCED.asp) によって公開されています。発行されたスプレッドシートの変換は、起動時に解析される `Vocabulary/sced-v4.csv` ファイルに含まれます。

### カスタマイズされたシリアル化

[規定された JSON バインディング](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452033)を一致させるために、すべての応答 JSON オブジェクトは NewtonSoft JSON NuGet パッケージを使用して記述されます。

サービス エンドポイント応答にシリアル化することが可能なすべてのエンティティでは、`AsJson` と `AsJsonReference` の 2 つのシリアル化メソッドがサポートされています。

**AsJson** は、エンティティがプライマリ エンティティまたは要求されたプライマリ コレクションのメンバーである場合に使用されます。

**AsJsonReference** は、エンティティを [GUIDRef](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452031) として表すために使用されます。

### ページ処理、フィルター処理、並べ替え

`Controllers/BaseController` クラスは、[ページ処理、フィルター処理、並べ替え](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451994)にフック インします。

すべてのデータ フィールドはフィルター処理または並べ替えに使用できる必要があるという要件をサポートするために、フィルター処理と並べ替えは、モデル クラスのリフレクションによって実装されます。

### データ ソース

SQLite データベースが想定され、`Startup.cs` で構成されます。

パッケージ マネージャー コンソールで `EntityFrameworkCore\Update-Database` コマンドを実行して新しい空のデータベースを作成することができ、`/seeds` エンドポイントにアクセスすることによりそのデータベースにシードを投入できます。

## サポートされている OneRoster エンドポイント

OneRoster v1.1 サービス サブセットは 3 つあり、[こちら](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989)で定義されています。このうち、このサンプルでは Rostering (読み取り専用) サブセットと Gradebook (読み取りと書き込み) サブセットが実装されます。

### Rostering のエンドポイント (読み取り)

| サービスの呼び出し | エンドポイント | HTTP 動詞 |
|--------------------------------|-----------------------------------------------------|-----------|
| getAllAcademicSessions | /academicSessions | GET |
| getAcademicSession | /academicSessions/{id} | GET |
| getAllClasses | /classes | GET |
| getClass | /classes/{id} | GET |
| getStudentsForClass | /classes/{class_id}/students | GET |
| getTeachersForClass | /classes/{class_id}/teachers | GET |
| getAllCourses | /courses | GET |
| getCourse | /courses/{id} | GET |
| getClassesForCourse | /courses/{course_id}/classes | GET |
| getAllDemographics | /demographics | GET |
| getDemographics | /demographics/{id} | GET |
| getAllEnrollments | /enrollments | GET |
| getEnrollment | /enrollments/{id} | GET |
| getAllGradingPeriods | /gradingPeriods | GET |
| getGradingPeriod | /gradingPeriods/{id} | GET |
| getAllOrgs | /orgs | GET |
| getOrg | /orgs/{id} | GET |
| getAllSchools | /schools | GET |
| getSchool | /schools/{id} | GET |
| getCoursesForSchool | /schools/{id}/courses | GET |
| getClassesForSchool | /schools/{school_id}/classes | GET |
| getEnrollmentsForSchool | /schools/{school_id}/enrollments | GET |
| getEnrollmentsForClassInSchool | /schools/{school_id}/classes/class_id}/enrollments | GET |
| getStudentsForClassInSchool | /schools/{school_id}/classes/{class_id}/students | GET |
| getStudentsForSchool | /schools/{school_id}/students | GET |
| getTeachersForClassInSchool | /schools/{school_id}/classes/{class_id}/teachers | GET |
| getTeachersForSchool | /schools/{school_id}/teachers | GET |
| getTermsForSchool | /schools/{school_id}/terms | GET |
| getAllStudents | /students | GET |
| getStudent | /students/{id} | GET |
| getClassesForStudent | /students/{id}/classes | GET |
| getAllTeachers | /teachers | GET |
| getTeacher | /teachers/{id} | GET |
| getClassesForTeacher | /teachers/{id}/classes | GET |
| getAllTerms | /terms | GET |
| getTerm | /terms/{id} | GET |
| getClassesForTerm | /terms/{id}/classes | GET |
| getGradingPeriodsForTerm | /terms/{id}/gradingPeriods | GET |
| getAllUsers | /users | GET |
| getUser | /users/{id} | GET |
| getClassesForUser | /users/{id}/classes | GET |


### Gradebook のエンドポイント (読み取り)

| サービスの呼び出し | エンドポイント | HTTP 動詞 |
|-------------------------------|---------------------------------------------------|-----------|
| getAllCategories | /categories | GET |
| getCategory | /categories/{id} | GET |
| getAllLineItems | /lineItems | GET |
| getLineItem | /lineItems/{id} | GET |
| getAllResults | /results | GET |
| getResult | /results/{id} | GET |
| getResultsForClass | /classes/{class_id}/results | GET |
| getLineItemsForClass | /classes/{class_id}/lineItems | GET |
| getResultsForLineItemForClass | /classes/{class_id}/lineItems/{li_id}/results | GET |
| getResultsForStudentForClass | /classes/{class_id}/students/{student_id}/results | GET |

### Gradebook のエンドポイント (書き込み)

| サービスの呼び出し | エンドポイント | HTTP 動詞 |
|-------------------------------|---------------------------------------------------|-----------|
| deleteCategory | /categories/{id} | DELETE |
| putCategory | /categories/{id} | PUT |
| deleteLineItem | /lineItems/{id} | DELETE |
| putLineItem | /lineItems/{id} | PUT |
| deleteResult | /results/{id} | DELETE |
| putResult | /results/{id} | PUT |

## サポート対象外の機能

現在、次の動作は仕様を満たしていません。

* [入れ子のプロパティのフィルター処理](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451997)
* [フィールドの選択](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451998)

## 質問とコメント

* このサンプルの実行で問題が発生した場合は、[問題を報告](https://github.com/OfficeDev/O365-EDU-SDS-AspNetMVC-Samples/issues)してください。
* ASP Core の開発に関する全般的な質問は、「[Stack Overflow](https://stackoverflow.com/questions/tagged/asp.net-core)」に投稿してください。質問やコメントには、必ず [asp.net-core] とタグを付けてください。
* OneRoster の開発に関する全般的な質問は、「[IMS Global](https://www.imsglobal.org/forums/ims-glc-public-forums-and-resources/oneroster-public-forum)」に投稿してください。

## 投稿

このプロジェクトは投稿や提案を歓迎します。
たいていの投稿には、投稿者のライセンス契約 (CLA) に同意することにより、投稿内容を使用する権利を Microsoft に付与する権利が自分にあり、実際に付与する旨を宣言していただく必要があります。
詳細については、https://cla.microsoft.com をご覧ください。

プル要求を送信すると、CLA を提供して PR を適切に修飾する (ラベル、コメントなど) 必要があるかどうかを CLA ボットが自動的に判断します。
ボットの指示に従ってください。
すべてのリポジトリに対して 1 度のみ、CLA を使用してこれを行う必要があります。

このプロジェクトでは、[Microsoft Open Source Code of Conduct (Microsoft オープン ソース倫理規定)](https://opensource.microsoft.com/codeofconduct/) が採用されています。
詳細については、「[倫理規定の FAQ](https://opensource.microsoft.com/codeofconduct/faq/)」を参照してください。また、
その他の質問やコメントがあれば、[opencode@microsoft.com](mailto:opencode@microsoft.com) までお問い合わせください。



**Copyright (c) 2018 Microsoft.All rights reserved.**
