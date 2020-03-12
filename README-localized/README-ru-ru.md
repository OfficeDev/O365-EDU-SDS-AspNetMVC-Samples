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
description: "Этот проект реализует поведение OneRoster v1.1"
---

# Образец поставщика OneRoster REST

Этот проект реализует поведение OneRoster v1.1.

В частности, это требование для:

1. OneRoster v1.1 Rostering REST Provider
2. OneRoster v1.1 Gradebook REST Provider

**Оглавление**
* [Примеры целей](#sample-goals)
* [Предварительные требования](#prerequisites)
* [Сборка и отладка в локальной среде](#build-and-debug-locally)
* [Развертывание примера в Azure](#deploy-the-sample-to-azure)
* [Синхронизация с SDS](#sync-with-sds)
* [Понимание кода](#understand-the-code)
* [Поддерживаемые конечные точки OneRoster](#supported-oneroster-endpoints)
* [Неподдерживаемые возможности](#unsupported-features)
* [Вопросы и комментарии](#questions-and-comments)
* [Помощь](#contributing)

## Примеры целей

В примере показано:

* Модели данных, поддерживающие [объекты OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006)
* Контроллеры действий, поддерживающие [конечные точки службы OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989)
* ПО промежуточного слоя проверки подлинности [основ безопасности OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452001)
* Создание [CSV-пакетов OneRoster](https://www.imsglobal.org/oneroster-v11-final-csv-tables)
* Использование [API управления профилем SDS](https://github.com/OfficeDev/O365-EDU-Tools/tree/master/SDSProfileManagementDocs) для создания и запуска профилей синхронизации SDS

OneRosterProviderDemo основан на ASP.NET Core Web.

## Необходимые компоненты

**Для развертывания и запуска этого примера требуется следующее**:

* Подписка Azure с разрешениями на регистрацию нового приложения и развертывание веб-приложения.
* Клиент Office 365 для образования с поддержкой Microsoft School Data Sync.
* Один из следующих браузеров: Microsoft Edge, Internet Explorer 9, Safari 5.0.6, Firefox 5, Chrome 13 или более поздние версии этих браузеров.
* Средство создания подписей OAuth1, например [Postman](https://www.getpostman.com/)
* Visual Studio 2017 (любого выпуска), [Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community) доступно бесплатно.
* Общее представление о C#, приложениях .NET Web и веб-службах.

## Регистрация приложения в Azure Active Directory

1. Войдите на новый портал Azure по адресу [https://portal.azure.com](https://portal.azure.com/).

2. Щелкните **Azure Active Directory** -> **Регистрация приложений** -> **+Добавить**.

   ![](Images/aad-create-app-01.png)

3. Введите **Имя** и выберите **Веб-приложение или API** в качестве **типа приложения**.

   Укажите **URL-адрес входа**: https://localhost:44344/

   ![](Images/aad-create-app-02.png)

   Нажмите **Создать**.

4. После завершения приложение отобразится в списке.

   ![](/Images/aad-create-app-03.png)

5. Щелкните его, чтобы просмотреть сведения о нем.

   ![](/Images/aad-create-app-04.png)

6. Щелкните **Все параметры**, если окно параметров не отобразилось.

   * Нажмите **Свойства** и присвойте параметру **Мультитенантный** значение **Да**.

     Скопируйте **Идентификатор приложения** и нажмите кнопку **Сохранить**.

   * Щелкните **Необходимые разрешения**. Добавьте указанные ниже разрешения.

     | API | Делегированные разрешения |
| ------------------------------ | ---------------------------------------- |
| Microsoft Graph | Чтение данных каталога<br>Чтение и запись данных каталога<br>Доступ к каталогу, аналогичный доступу вошедшего пользователя<br>Чтение настроек обучающего приложения<br>Управление настройками обучающего приложения |
| Windows Azure Active Directory |
| Вход в систему и чтение профиля пользователя

   * Щелкните пункт **Ключи** и добавьте новый ключ:

     ![](Images/aad-create-app-07.png)

     Нажмите **Сохранить**, затем скопируйте **ЗНАЧЕНИЕ** ключа.

   Закройте окно параметров.

## Сборка и отладка в локальной среде

Этот проект можно открыть с помощью выпуска Visual Studio 2017, который у вас уже есть, или скачать и установить выпуск Community, чтобы запускать, создавать или разрабатывать это приложение в локальной среде.

- [Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community)

Отладка **OneRosterProviderDemo**:

1. Настойка **appsettings.json**.

   ![](Images/web-app-config.png)

   - **AzureAd:Authority**: "https://login.microsoftonline.com/{your-ad-tenant}.onmicrosoft.com/"
   - **AzureAd:ClientId**: используйте идентификатор клиента из регистрации приложения, созданный ранее.
   - **AzureAd:ClientSecret**: используйте значение ключа из регистрации приложения, созданное ранее.
   - **AzureDomain**: используйте домен клиента AD, который согласовывается с записью **AzureAd:Authority**

2. Выполните команду `EntityFrameworkCore\Update-Database` в консоли диспетчера пакетов, чтобы создать исходную базу данных. Если это приведет к ошибке, попробуйте выполнить команду `Import-Package Microsoft.EntityFrameworkCore`.
3. Установите **OneRosterProviderDemo** в качестве запускаемого проекта, а затем нажмите F5.
4. Посетите `/seeds` для заполнения базы данных образцами объектов. Убедитесь в наличии доступа к конечной точке с помощью HTTPS.

## Развертывание примера в Azure

1. Создайте профиль публикации

  - Выберите **Сборка > Опубликовать OneRosterProviderDemo**
  - Щелкните **Создать новый профиль**.
  - Выберите **Служба приложений Microsoft Azure** и **Создать**
  - Нажмите кнопку **Опубликовать**
  - Войдите в учетную запись Azure.
  - Выберите группу ресурсов для развертывания или создайте новую, нажав кнопку **Новое**
  - Выберите план службы приложений для развертывания или создайте новый, нажав кнопку **Новое**
  - Нажмите **Create**

2. Выберите **Сборка > Опубликовать OneRosterProviderDemo**
3. Нажмите кнопку **Опубликовать**

**Добавление URL-адреса ответа в регистрацию приложения**

1. После развертывания откройте группу ресурсов в портале Azure:
2. Щелкните веб-приложение.

   ![](Images/azure-web-app.png)

   Скопируйте URL-адрес и измените схему на **https**. Добавьте **/signin-oidc** в конец. Это URL-адрес ответа, который будет использоваться в следующем действии.

3. Перейдите к регистрации приложения на новом портале Azure и откройте окно параметров.

   Добавьте URL-адрес ответа:

   ![](Images/aad-add-reply-url.png)

   > Примечание. Чтобы выполнить отладку примера локально, укажите https://localhost:44344/signin-oidc в URL-адресах ответа.

4. Нажмите кнопку **Сохранить**.

## Синхронизация с SDS

Возможны два варианта синхронизации: через конечные точки OneRoster REST и путем отправки [CSV-файлов, совместимых с SDS](https://support.office.com/en-us/article/CSV-files-for-School-Data-Sync-9f3c3c2b-7364-4f6e-a959-e8538feead70).

### Синхронизация с OneRoster REST
1. Посетите страницу развертывания на /sds
2. Войдите с помощью учетной записи администратора в своем клиенте для образовательных учреждений.
3. Убедитесь, что значение "конечной точки OneRoster" согласуется с развернутым экземпляром
  - Обратите внимание на то, что это не сработает, если URL-адрес не общедоступен
4. Нажмите кнопку "Синхронизировать с помощью OneRoster REST"

### Синхронизация с CSV-файлами в SDS
1. Посетите страницу развертывания на /sds
2. Войдите с помощью учетной записи администратора в своем клиенте для образовательных учреждений.
3. Выберите CSV-файлы в SDS
4. Нажмите кнопку "Синхронизовать с помощью CSV-файлов в SDS".

## Разбор кода

### Введение

Это веб-приложение основано на шаблоне проекта ASP.NET Core Web.

### Авторизация

#### Конечные точки OneRoster
Файл `Middlewares/OAuth.cs` определяет ПО промежуточного слоя, который проверяет подписи OAuth1 или OAuth2 для каждого входящего запроса с помощью маршрута OneRoster. Этот файл также содержит жестко закодированный идентификатор и секрет клиента. Для создания запросов OAuth2, посетите конечную точку /token, используя учетные данные OAuth1, чтобы получить маркер доступа. Файл `Startup.cs` настраивает приложение для использования ПО промежуточного слоя.

#### Управление профилями SDS
Файл `Startup.cs` настраивает приложение для использования библиотеки .NET Core's OpenIDConnect (oidc). Этот поток обрабатывается `AccountController`.

### Модели данных

Основные модели OneRoster имеют соответствующий класс в каталоге`Модель`. Учитывая соглашения об именовании, некоторое из следующего было переименовано. Сопоставление моделей указано в таблице ниже.

| Модель OneRoster | Модель EFCore |
|------------------------|-------------------|
| Основание | BaseModel |
| Академическая сессия | AcademicSession |
| Класс | IMSClass |
| Курс | Course |
| Демографические данные | Demographic |
| Регистрация | Enrollment |
| Элемент строки | LineItem |
| Категория элемента строки | LineItemCategory |
| Организация | Org |
| Ресурс | Resource |
| Результат | Result |
| Пользователь, ученик, преподаватель | User |

Дополнительные модели, созданные для поддержки различных задач:

| Модель EFCore | Назначение |
|-------------------------|-------------------------------------------------|
| IMSClassAcademicSession | Присоединяет таблицу "Класс", "Срок (академическая сессия)" |
| UserAgent | Присоединяет таблицу "Пользователь", "Пользователь"|
| UserOrg | Присоединяет таблицу "Пользователь", "Организация" |
| UserId | Форма идентификаторов пользователей |
| SeedData | Содержит начальные данные для заполнения исходной базы данных |
| OauthNonce | Хранит значения nonce, для которых запрещено повторное использование |

### Проверка

Требования к проверке определены в спецификации [модели OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006).

В дополнение к атрибутам проверки, переданным .NET и ASP Core, существует четыре настраиваемых элемента проверки для свойств с определенными требованиями в папке `Средства проверки`.

#### Словарь
[Словари](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452048) используемые OneRoster, включая [перечисления](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452020), предоставляются классом `Vocabulary/Vocabulary`.

Словарь кода темы "SCEDv4.0" публикуется [NCES](https://nces.ed.gov/forum/SCED.asp). Преобразование опубликованных электронных таблиц включено в файл `Vocabulary/sced-v4.csv`, который анализируется во время запуска.

### Настраиваемая сериализация

Все объекты ответов JSON записываются с помощью пакета NewtonSoft JSON nuget, в целях соответствия [предписанных привязок JSON](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452033).

Все объекты, которые можно сериализовать в ответ конечной точки службы, поддерживают два способа сериализации: `AsJson` и `AsJsonReference`.

**AsJson** используется, когда объект является основным или состоит в главной запрошенной коллекции.

**AsJsonReference** используется для представления объекта в виде [GUIDRef](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452031).

### Разбивка на страницы, Фильтрация и Сортировка

Класс `Controllers/BaseController` задействован в [разбивке на страницы, фильтрации и сортировке](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451994).

Фильтрация и сортировка реализуются в классах модели с помощью отражения, чтобы обеспечить обязательное использование любых полей данных для фильтрации и сортировки.

### Источник данных

Предполагается, что база данных SQLite настраивается в `Startup.cs`.

Вы можете создать новую, пустую базу данных, запустив команду `EntityFrameworkCore\Update-Database` в консоли диспетчера пакетов и указав в ней образец данных, посетив конечную точку `/seeds`.

## Поддерживаемые конечные точки OneRoster

Три подмножества служб OneRoster v 1.1, указаны [тут](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989). В этом примере реализованы подмножества Списков (только для чтения) и Журнала успеваемости (чтение и запись).

### Конечные точки Списков (Чтение)

| Вызов службы | Конечная точка | HTTP Verb |
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


### Конечные точки Журнала Успеваемости (Чтение)

| Вызов службы | Конечная точка | HTTP Verb |
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

### Конечные точки Журнала Успеваемости (Запись)

| Вызов службы | Конечная точка | HTTP Verb |
|-------------------------------|---------------------------------------------------|-----------|
| deleteCategory | /categories/{id} | DELETE |
| putCategory | /categories/{id} | PUT |
| deleteLineItem | /lineItems/{id} | DELETE |
| putLineItem | /lineItems/{id} | PUT |
| deleteResult | /results/{id} | DELETE |
| putResult | /results/{id} | PUT |

## Неподдерживаемые возможности

В настоящее время следующие варианты поведения не относятся к спецификациям.

* [Фильтрация вложенных свойств](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451997)
* [Выбор поля](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451998)

## Вопросы и комментарии

* Если у вас возникли проблемы с запуском этого примера, [сообщите о неполадке](https://github.com/OfficeDev/O365-EDU-SDS-AspNetMVC-Samples/issues).
* Общие вопросы о разработке ASP Core следует задавать на сайте [Stack Overflow](https://stackoverflow.com/questions/tagged/asp.net-core). Пометьте вопросы и комментарии тегом [asp.net-core].
* Общие вопросы о разработке OneRoster следует задавать на сайте [IMS Global](https://www.imsglobal.org/forums/ims-glc-public-forums-and-resources/oneroster-public-forum).

## Участие

Мы всегда рады предложениям и помощи в работе над проектом.
Обычно для добавления своих вариантов необходимо принять Лицензионное соглашение с участником (CLA), заявив, что вы имеете право предоставлять и предоставляете нам права на использование своего варианта.
Подробности см. на сайте https://cla.microsoft.com.

Когда вы будете отправлять запрос на вытягивание, CLA-бот автоматически определит, нужно ли вам предоставить CLA и соответствующим образом изменит внешний вид запроса на вытягивание (например, добавит метку, комментарий).
Просто следуйте инструкциям бота.
Это нужно будет сделать всего один раз для добавления своих вариантов во все репозитории, использующие наш CLA.

Этот проект соответствует [Правилам поведения разработчиков открытого кода Майкрософт](https://opensource.microsoft.com/codeofconduct/).
Дополнительные сведения см. в разделе [часто задаваемых вопросов о правилах поведения](https://opensource.microsoft.com/codeofconduct/faq/).
Если у вас возникли вопросы или замечания, напишите нам по адресу [opencode@microsoft.com](mailto:opencode@microsoft.com).



**(c) Корпорация Майкрософт (Microsoft Corporation), 2018. Все права защищены.**
