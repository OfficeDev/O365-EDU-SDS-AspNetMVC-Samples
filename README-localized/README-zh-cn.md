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
description: "此项目实现 OneRoster v1.1 行为。"
---

# OneRoster REST 提供程序示例

此项目实现 OneRoster v1.1 行为。

具体而言，这是针对以下对象的要求：

1. OneRoster v1.1 名册 REST 提供程序
2. OneRoster v1.1 成绩簿 REST 提供程序

**目录**
* [示例目标](#sample-goals)
* [先决条件](#prerequisites)
* [在本地构建和调试](#build-and-debug-locally)
* [将示例部署到 Azure](#deploy-the-sample-to-azure)
* [使用 SDS 进行同步](#sync-with-sds)
* [了解代码](#understand-the-code)
* [支持的 OneRoster 终结点](#supported-oneroster-endpoints)
* [不受支持的功能](#unsupported-features)
* [问题和意见](#questions-and-comments)
* [参与](#contributing)

## 示例目标

本示例演示：

* 支持 [OneRoster 实体](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006)的数据模型
* 支持 [OneRoster 服务终结点](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989)的操作控制器
* 支持 [OneRoster 核心安全性](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452001)的授权中间件
* 生成 [OneRoster CSV 捆绑包](https://www.imsglobal.org/oneroster-v11-final-csv-tables)
* 使用 [SDS 配置文件管理 API](https://github.com/OfficeDev/O365-EDU-Tools/tree/master/SDSProfileManagementDocs) 创建和启动 SDS 同步配置文件

OneRosterProviderDemo 基于 ASP.NET Core Web。

## 先决条件

**部署和运行此示例需要**：

* 具有注册新应用、部署 Web 应用权限的 Azure 订阅。
* 启用了 Microsoft School Data Sync 的 O365 教育版租户
* 下列一款浏览器：Microsoft Edge、Internet Explorer 9、Safari 5.0.6、Firefox 5、Chrome 13 或这些浏览器的更高版本。
* 用于生成 OAuth1 签名的工具，如 [Postman](https://www.getpostman.com/)
* Visual Studio 2017（任何版本），[Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community) 免费提供。
* 熟悉 C#、.NET Web 应用程序和 Web 服务。

## 在 Azure Active Directory 中注册应用程序

1. 登录新的 Azure 门户：[https://portal.azure.com/](https://portal.azure.com/)。

2. 单击“**Azure Active Directory**” -> “**应用注册** -> “**+添加**”。

   ![](Images/aad-create-app-01.png)

3. 输入“**名称**”，并选择“**Web 应用/API**”作为“**应用程序类型**”。

   输入**登录 URL**：https://localhost:44344/

   ![](Images/aad-create-app-02.png)

   单击“**创建**”。

4. 完成后，应用程序在列表中显示。

   ![](/Images/aad-create-app-03.png)

5. 单击可查看详细信息。

   ![](/Images/aad-create-app-04.png)

6. 如果设置窗口未显示，单击“**所有设置**”。

   * 单击“**属性**”，然后将“**多租户**”设置为“**是**”。

     复制“**应用程序 ID**”，随后单击“**保存**”。

   * 单击“**必需的权限**”。添加以下权限：

     | API | 委派权限 |
| ------------------------------ | ---------------------------------------- |
| Microsoft Graph | 读取目录数据<br>读取和写入目录数据<br>以登录用户身份访问目录<br>读取教育应用设置<br>管理教育应用设置 |
| Windows Azure Active Directory | 登录和读取用户个人资料 |

   * 单击“**密钥**”，随后添加新密钥：

     ![](Images/aad-create-app-07.png)

     单击“**保存**”，随后复制密钥“**值**”。

   关闭“设置”窗口。

## 在本地构建和调试

可使用已有的 Visual Studio 2017 版本打开此项目，也可以下载并安装 Community 版以在本地运行、构建和/或开发此应用程序。

- [Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community)

调试 **OneRosterProviderDemo**：

1. 配置 **appsettings.json**。

   ![](Images/web-app-config.png)

   - **AzureAd:Authority**：“https://login.microsoftonline.com/{your-ad-tenant}.onmicrosoft.com/”
   - **AzureAd:ClientId**：使用之前创建的应用注册客户端 ID。
   - **AzureAd:ClientSecret**：使用之前创建的应用注册密钥值。
   - **AzureDomain**：使用你的 AD 租户域，它应该与 **AzureAd:Authority** 实体相一致

2. 在程序包管理器控制台中，运行 `EntityFrameworkCore\Update-Database` 命令以生成初始数据库。如果这导致出现错误，请尝试运行 `Import-Package Microsoft.EntityFrameworkCore` 命令。
3. 将 **OneRosterProviderDemo** 设置为，然后按 F5。
4. 访问 `/seeds` 以使用示例实体填充你的数据库。确保使用 HTTPS 访问终结点。

## 将示例部署到 Azure

1. 创建发布配置文件

  - 选择“**构建 > 发布 OneRosterProviderDemo**”
  - 单击“**创建新配置文件**”
  - 选择“**Microsoft Azure 应用服务**”和“**新建**”
  - 单击“**发布**”
  - 登录你的 Azure 帐户
  - 选择要部署的资源组，或单击“**新建**”以创建一个新资源组
  - 选择要部署的应用服务计划，或单击“**新建**”以创建一个新计划
  - 单击“**创建**”

2. 选择“**构建 > 发布 OneRosterProviderDemo**”
3. 单击“**发布**”

**添加回复 URL 至应用程序注册**

1. 部署后，在 Azure 门户中打开资源组
2. 单击 Web 应用。

   ![](Images/azure-web-app.png)

   复制 URL 并将架构更改为 **https**，然后在末尾添加 **/signin-oidc**。这是回复 URL，将在下一步中使用。

3. 导航至新 Azure 门户中的应用程序注册，随后打开设置窗口。

   添加回复 URL：

   ![](Images/aad-add-reply-url.png)

   > 注意：如果要在本地调试示例，确保回复 URL 是 https://localhost:44344/signin-oidc。

4. 单击“**保存**”。

## 使用 SDS 进行同步

有两个同步选项，即通过 OneRoster REST 终结点和上传 [SDS 兼容 CSV 文件](https://support.office.com/en-us/article/CSV-files-for-School-Data-Sync-9f3c3c2b-7364-4f6e-a959-e8538feead70)。

### 使用 OneRoster REST 进行同步
1. 访问 /sds 上的部署
2. 使用教育版租户上的管理员帐户登录。
3. 验证“OneRoster 终结点”值是否与你部署的实例相一致
  - 请注意，除非你的 URL 可公开访问，否则这将无法工作
4. 单击“通过 OneRoster REST 同步”按钮

### 使用 SDS CSV 进行同步
1. 访问 /sds 上的部署
2. 使用教育版租户上的管理员帐户登录。
3. 选择你的 SDS CSV 文件
4. 单击“通过 SDS CSV 同步”按钮

## 了解代码

### 简介

此 Web 应用程序基于 ASP.NET Core Web 项目模板。

### 授权

#### OneRoster 终结点
`Middlewares/OAuth.cs` 文件定义了一个中间件，该中间件使用 OneRoster 路由为每个传入请求验证 OAuth1 或 OAuth2 签名。此文件还包含硬编码的客户端 ID 和密码。为了发起 OAuth2 请求，请首先使用 OAuth1 凭据访问 /token 终结点以获取访问令牌。`Startup.cs` 文件将应用配置为使用此中间件。

#### SDS 配置文件管理
`Startup.cs` 文件将应用配置为使用 .NET Core 的 OpenIDConnect (oidc) 库。此流由 `AccountController` 进行处理。

### 数据模型

大多数 OneRoster 模型在 `Models` 目录中都有对应的模型类。由于语言命名约定，其中一些已重命名。下表显示了模型的映射。

| OneRoster 模型 | EFCore 模型 |
|------------------------|-------------------|
| 基本 | BaseModel |
| 学术会议 | AcademicSession |
| 班级 | IMSClass |
| 课程 | 课程 |
| 人口统计数据 | 人口统计 |
| 注册 | 注册 |
| 行项 | LineItem |
| 行项类别 | LineItemCategory |
| 组织 | 组织 |
| 资源 | 资源 |
| 结果 | 结果 |
| 用户、学生、教师 | 用户 |

创建了其他模型以支持各种需求：

| EFCore 模型 | 用途 |
|-------------------------|-------------------------------------------------|
| IMSClassAcademicSession | 班级、学期（学术会议）的联接表 |
| UserAgent | 用户、用户的联接表 |
| UserOrg | 用户、组织的联接表 |
| UserId | 用户 ID 的形状 |
| SeedData | 保留初始数据库填充的种子数据 |
| OauthNonce | 存储 nonce 值以禁用重复使用 |

### 验证

[OneRoster 模型规范](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006)中定义了验证要求。

除了 .NET 和 ASP Core 提供的验证属性外，`Validators` 文件夹中还有四个针对具有特定要求的属性的自定义验证程序。

#### 词汇表
OneRoster 使用的[词汇表](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452048)（包括[枚举](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452020)）由 `Vocabulary/Vocabulary` 类提供。

主题代码词汇“SCEDv4.0”由 [NCES](https://nces.ed.gov/forum/SCED.asp) 发布；`Vocabulary/sced-v4.csv` 文件中存在已发布电子表格的转换，该文件在启动时进行了解析。

### 自定义序列化

所有响应 json 对象都是使用 NewtonSoft JSON nuget 程序包编写的，目的是匹配[规定的 json 绑定](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452033)。

可序列化为服务终结点响应的所有实体都支持两种序列化方法，即 `AsJson` 和 `AsJsonReference`。

当实体是所请求的主实体或主集合的成员时，将使用 **AsJson**。

**AsJsonReference** 用于将实体表示为 [GUIDRef](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452031)。

### 分页、筛选和排序

`Controllers/BaseController` 与[分页、筛选和排序](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451994)挂钩。

筛选和排序通过反射在模型类中实现，以支持任何数据字段都可用于筛选或排序的要求。

### 数据源

在 `Startup.cs` 中假定并配置 SQLite 数据库。

可以通过在程序包管理器控制台中运行 `EntityFrameworkCore\Update-Database` 命令来创建一个新的空数据库，并通过访问 `/seeds` 终结点和使用示例数据为其设定种子。

## 支持的 OneRoster 终结点

[此处](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989)定义了三个 OneRoster v1.1 服务子集。其中，名册（只读）和成绩簿（读写）子集由该示例实现。

### 名册终结点（读取）

| 服务调用 | 终结点 | HTTP 谓词 |
|--------------------------------|-----------------------------------------------------|-----------|
| getAllAcademicSessions | /academicSessions | GET |
| getAcademicSession | /academicSessions/{id} | GET |
| getAllClasses | /classes | GET |
| getClass | /classes/{id} | GET |
| getStudentsForClass | /classes/{class\_id}/students | GET |
| getTeachersForClass | /classes/{class\_id}/teachers | GET |
| getAllCourses | /courses | GET |
| getCourse | /courses/{id} | GET |
| getClassesForCourse | /courses/{course\_id}/classes | GET |
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
| getClassesForSchool | /schools/{school\_id}/classes | GET |
| getEnrollmentsForSchool | /schools/{school\_id}/enrollments | GET |
| getEnrollmentsForClassInSchool | /schools/{school\_id}/classes/class\_id}/enrollments | GET |
| getStudentsForClassInSchool | /schools/{school\_id}/classes/{class\_id}/students | GET |
| getStudentsForSchool | /schools/{school\_id}/students | GET |
| getTeachersForClassInSchool | /schools/{school\_id}/classes/{class\_id}/teachers | GET |
| getTeachersForSchool | /schools/{school\_id}/teachers | GET |
| getTermsForSchool | /schools/{school\_id}/terms | GET |
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


### 成绩簿终结点（读取）

| 服务调用 | 终结点 | HTTP 谓词 |
|-------------------------------|---------------------------------------------------|-----------|
| getAllCategories | /categories | GET |
| getCategory | /categories/{id} | GET |
| getAllLineItems | /lineItems | GET |
| getLineItem | /lineItems/{id} | GET |
| getAllResults | /results | GET |
| getResult | /results/{id} | GET |
| getResultsForClass | /classes/{class\_id}/results | GET |
| getLineItemsForClass | /classes/{class\_id}/lineItems | GET |
| getResultsForLineItemForClass | /classes/{class\_id}/lineItems/{li\_id}/results | GET |
| getResultsForStudentForClass | /classes/{class\_id}/students/{student\_id}/results | GET |

### 成绩簿终结点（写入）

| 服务调用 | 终结点 | HTTP 谓词 |
|-------------------------------|---------------------------------------------------|-----------|
| deleteCategory | /categories/{id} | DELETE |
| putCategory | /categories/{id} | PUT |
| deleteLineItem | /lineItems/{id} | DELETE |
| putLineItem | /lineItems/{id} | PUT |
| deleteResult | /results/{id} | DELETE |
| putResult | /results/{id} | PUT |

## 不受支持的功能

目前，以下行为不是最新规范：

* [嵌套属性筛选](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451997)
* [字段选择](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451998)

## 问题和意见

* 如果你在运行此示例时遇到任何问题，请[记录问题](https://github.com/OfficeDev/O365-EDU-SDS-AspNetMVC-Samples/issues)。
* 与 ASP Core 开发相关的问题一般应发布到 [Stack Overflow](https://stackoverflow.com/questions/tagged/asp.net-core)。确保你的问题或意见使用 \[asp.net-core] 进行了标记。
* 与 OneRoster 开发相关的问题一般应发布到 [IMS Global](https://www.imsglobal.org/forums/ims-glc-public-forums-and-resources/oneroster-public-forum)。

## 参与

本项目欢迎供稿和建议。
大多数的供稿都要求你同意“参与者许可协议 (CLA)”，声明你有权并确定授予我们使用你所供内容的权利。
有关详细信息，请访问 https://cla.microsoft.com。

在提交拉取请求时，CLA 机器人会自动确定你是否需要提供 CLA 并适当地修饰 PR（例如标记、批注）。
只需按照机器人提供的说明操作即可。
只需在所有存储库上使用我们的 CLA 执行此操作一次。

此项目已采用 [Microsoft 开放源代码行为准则](https://opensource.microsoft.com/codeofconduct/)。
有关详细信息，请参阅[行为准则 FAQ](https://opensource.microsoft.com/codeofconduct/faq/)。
如有其他任何问题或意见，也可联系 [opencode@microsoft.com](mailto:opencode@microsoft.com)。



**版权所有 (c) 2018 Microsoft。保留所有权利。**
