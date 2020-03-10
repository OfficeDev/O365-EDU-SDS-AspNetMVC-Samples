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
description: "This project implements OneRoster v1.1 behaviors."
---

# Ejemplo de proveedor de REST de OneRoster

Este proyecto implementa comportamientos de OneRoster v1.1.

En concreto, se enfoca en los requisitos para lo siguiente:

1. Proveedor de REST de generación de listados de OneRoster v 1.1
2. Proveedor de REST de libro de calificaciones de OneRoster v 1.1

**Tabla de contenido**
* [Objetivos del ejemplo](#sample-goals)
* [Requisitos previos](#prerequisites)
* [Compilar y depurar localmente](#build-and-debug-locally)
* [Implementar el ejemplo en Azure](#deploy-the-sample-to-azure)
* [Sincronizar con SDS](#sync-with-sds)
* [Entender el código](#understand-the-code)
* [Puntos de conexión de OneRoster compatibles](#supported-oneroster-endpoints)
* [Características no compatibles](#unsupported-features)
* [Preguntas y comentarios](#questions-and-comments)
* [Contribuciones](#contributing)

## Objetivos del ejemplo

El ejemplo demuestra lo siguiente:

* Modelos de datos compatibles con [entidades de OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006)
* Controladores de acciones compatibles con los [puntos de conexión de servicio de OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989)
* Middleware de autorización compatible con la [seguridad esencial de OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452001)
* Generación de [paquetes CSV de OneRoster](https://www.imsglobal.org/oneroster-v11-final-csv-tables)
* Consumo de las [API de administración de perfiles de SDS](https://github.com/OfficeDev/O365-EDU-Tools/tree/master/SDSProfileManagementDocs) para crear e iniciar perfiles de sincronización de SDS

OneRosterProviderDemo se basa en ASP.NET Core Web.

## Requisitos previos

**Implementar y ejecutar este ejemplo requiere lo siguiente**:

* Una suscripción de Azure con permisos para registrar una nueva aplicación y para implementar la aplicación web.
* Un inquilino de O365 Educación con Microsoft School Data Sync habilitado.
* Uno de los siguientes exploradores: Microsoft Edge, Internet Explorer 9, Safari 5.0.6, Firefox 5, Chrome 13 o una versión posterior de cualquiera de ellos.
* Una herramienta para generar firmas OAuth1, como [Postman](https://www.getpostman.com/).
* Visual Studio 2017 (cualquier edición); [Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community) está disponible de forma gratuita.
* Estar familiarizado con las aplicaciones y los servicios web de C# y .NET.

## Registrar la aplicación en Azure Active Directory

1. Inicie sesión en el nuevo portal de Azure: [https://portal.azure.com/](https://portal.azure.com/).

2. Haga clic en **Azure Active Directory** -> **Registros de aplicaciones** -> **+Agregar**.

   ![](Images/aad-create-app-01.png)

3. Escriba un **Nombre** y seleccione **Aplicación web o API** como **Tipo de aplicación**.

   Escriba https://localhost:44344/ en **URL de inicio de sesión**.

   ![](Images/aad-create-app-02.png)

   Haga clic en **Crear**.

4. Cuando haya finalizado, se mostrará la aplicación en la lista.

   ![](/Images/aad-create-app-03.png)

5. Haga clic en ella para ver los detalles.

   ![](/Images/aad-create-app-04.png)

6. Haga clic en **Configuración**, si no se muestra la ventana de configuración.

   * Haga clic en **Propiedades** y luego establezca el campo **Multiinquilino** en **Sí**.

     Copie aparte el **Id. de aplicación** y, después, haga clic en **Guardar**.

   * Haga clic en **Permisos necesarios**. Agregue los siguientes permisos:

     | API | Permisos delegados |
| ------------------------------ | ---------------------------------------- |
| Microsoft Graph | Read directory data<br>Read and write directory data<br>Access the directory as the signed in user<br>Read Education app settings<br>Manage education app settings |
| Windows Azure Active Directory | Sign in and read user profile |

   * Haga clic en **Claves** y agregue una nueva clave:

     ![](Images/aad-create-app-07.png)

     Haga clic en **Guardar** y copie aparte el **VALOR** de la clave.

   Cierre la ventana de configuración.

## Compilar y depurar localmente

Puede abrir este proyecto con la edición de Visual Studio 2017 que ya tiene, o puede descargar e instalar la edición de la comunidad para ejecutar, compilar y desarrollar esta aplicación localmente.

- [Visual Studio 2017 Community](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community)

Depurar **OneRosterProviderDemo**:

1. Configure **appsettings.json**.

   ![](Images/web-app-config.png)

   - **AzureAd:Authority**: "https://login.microsoftonline.com/{su-inquilino-de-ad}.onmicrosoft.com/".
   - **AzureAd:ClientId**: use el Id. de cliente del registro de la aplicación que creó anteriormente.
   - **AzureAd:ClientSecret**: use el valor de la clave del registro de la aplicación que creó anteriormente.
   - **AzureDomain**: use el dominio de inquilino de AD, que debe coincidir con la entrada **AzureAd: Authority**.

2. En la consola de administración de paquetes, ejecute el comando `EntityFrameworkCore\Update-Database` para generar la base de datos inicial. Si se produce un error, pruebe a ejecutar el comando `Import-Package Microsoft.EntityFrameworkCore`.
3. Establezca **OneRosterProviderDemo** como proyecto de inicio y presione F5.
4. Visit `/seeds` para rellenar la base de datos con entidades de ejemplo. Asegúrese de que obtiene acceso al punto de conexión mediante HTTPS.

## Implementar el ejemplo en Azure

1. Cree un perfil de publicación.

  - Seleccione **Compilación > Publicar OneRosterProviderDemo**.
  - Haga clic en **Crear nuevo perfil**.
  - Seleccione **Microsoft Azure App Service** y **Crear nuevo**.
  - Haga clic en **Publicar**.
  - Inicie sesión en su cuenta de Azure.
  - Seleccione un grupo de recursos para la implementación o cree uno nuevo haciendo clic en **Nuevo**.
  - Seleccione un Plan de servicio de la aplicación para la implementación o cree uno nuevo haciendo clic en **Nuevo**.
  - Haga clic en **Crear**.

2. Seleccione **Compilación > Publicar OneRosterProviderDemo**.
3. Haga clic en **Publicar**.

**Agregue una URL de respuesta al registro de la aplicación**.

1. Después de la implementación, abra el grupo de recursos en Azure Portal.
2. Haga clic en la aplicación web.

   ![](Images/azure-web-app.png)

   Copie la URL aparte y cambie el esquema a **https** y agregue **/signin-oidc** al final. Esta es la URL de respuesta y se utilizará en el siguiente paso.

3. Vaya al registro de aplicaciones del nuevo portal de Azure y abra las ventanas de configuración.

   Agregue la URL de respuesta:

   ![](Images/aad-add-reply-url.png)

   > Nota: Para depurar el ejemplo localmente, asegúrese de que https://localhost:44344/signin-oidc se encuentre en las URL de respuesta.

4. Haga clic en **GUARDAR**.

## Sincronizar con SDS

Hay dos opciones de sincronización, por medio de los puntos de conexión de REST de OneRoster y cargando los [archivos CSV compatibles con SDS](https://support.office.com/es-es/article/CSV-files-for-School-Data-Sync-9f3c3c2b-7364-4f6e-a959-e8538feead70).

### Sincronizar con REST de OneRoster
1. Visite la implementación en /sds.
2. Inicie sesión con una cuenta de administrador en su inquilino de educación.
3. Compruebe que el valor de "Punto de conexión de OneRoster" coincida con la instancia implementada.
  - Tenga en cuenta que esto no funcionará a menos que su URL sea accesible públicamente.
4. Haga clic en el botón "Sincronizar con REST de OneRoster".

### Sincronizar con CSV de SDS
1. Visite la implementación en /sds.
2. Inicie sesión con una cuenta de administrador en su inquilino de educación.
3. Seleccione los archivos CSV de SDS.
4. Haga clic en el botón "Sincronizar con CSV de SDS".

## Entender el código

### Introducción

Esta aplicación web está basada en una plantilla de proyecto de ASP.NET Core Web.

### Autorización

#### Puntos de conexión de OneRoster
El archivo `Middlewares/OAuth.cs` define un middleware que valida la firma OAuth1 o OAuth2 para cada solicitud entrante con una ruta de OneRoster.. Este archivo también contiene el Id. de cliente y el secreto codificados de forma rígida. Para realizar solicitudes de OAuth2, en primer lugar visite el punto de conexión /token con credenciales OAuth1 para obtener el token de acceso. El archivo `Startup.cs` configura la aplicación para que use este middleware.

#### Administración de perfiles de SDS
El archivo `Startup.cs` configura la aplicación para que use la biblioteca OpenIDConnect (oidc) de .NET Core. Este flujo está controlado por `AccountController`.

### Modelos de datos

La mayoría de los modelos de OneRoster tienen una clase de modelo correspondiente en el directorio `Models`. Debido a las convenciones de nomenclatura de idioma, se ha cambiado el nombre de algunos de ellos. La asignación de modelos se muestra en la tabla siguiente.

| Modelo OneRoster | Modelo EFCore |
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

Se crearon modelos adicionales para ser compatibles con las distintas necesidades:

| Modelo EFCore | Objetivo |
|-------------------------|-------------------------------------------------|
| IMSClassAcademicSession | Tabla combinada de Class, Term (Academic Session) |
| UserAgent | Tabla combinada de User, User |
| UserOrg | Tabla combinada de User, Org |
| UserId | Formato de los Id. de usuario |
| SeedData | Contiene los datos para la base de datos inicial |
| OauthNonce | Almacena valores nonce para no permitir la reutilización |

### Validación

Los requisitos de validación se definen en la [especificación del modelo OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006).

Además de los atributos de validación proporcionados por .NET y ASP Core, hay cuatro validadores personalizados para las propiedades con requisitos específicos en la carpeta `Validators`.

#### Vocabulario
Los [vocabularios](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452048) usados por OneRoster, incluidas las [enumeraciones](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452020), se proporcionan mediante la clase `Vocabulary/Vocabulary`.

El vocabulario de códigos de temas "SCEDv 4.0" es publicado por [NCES](https://nces.ed.gov/forum/SCED.asp); la conversión de la hoja de cálculo publicada se encuentra en el archivo `Vocabulary/sced-v4.csv`, que se analiza en el momento del inicio.

### Serialización personalizada

Todos los objetos JSON de respuesta se escriben con el paquete NuGet NewtonSoft JSON, con el fin de hacer coincidir los [enlaces JSON prescritos](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452033).

Todas las entidades que se pueden serializar en una respuesta de punto de conexión de servicio admiten dos métodos de serialización, `AsJson` y `AsJsonReference`.

**AsJson** se usa cuando la entidad es la entidad principal 
(o es un miembro de la colección principal) que se solicitó.

**AsJsonReference** se usa para expresar la entidad como una [GUIDRef](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452031).

### Paginación, filtrado y ordenación

La clase `Controllers/BaseController` enlaza la [paginación, el filtrado y la ordenación](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451994).

El filtrado y la ordenación se implementan en las clases de modelo mediante reflexión para admitir el requisito de que cualquier campo de datos pueda usarse para filtrar u ordenar.

### Origen de datos

Se asume que una base de datos SQLite está configurada en `Startup.cs`.

Para crear una nueva base de datos vacía, ejecute el comando `EntityFrameworkCore\Update-Database` en la consola del administrador de paquetes y, a continuación, inicialice el archivo con datos de ejemplo. Para ello, visite el punto de conexión `/seeds`.

## Puntos de conexión de OneRoster compatibles

Existen tres subconjuntos de servicios de OneRoster v 1.1, definidos [aquí](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989). De estos, los subconjuntos generación de listados (solo lectura) y libro de calificaciones (lectura y escritura) se implementan con este ejemplo.

### Puntos de conexión de generación de listados (lectura)

| Llamada de servicio | Punto de conexión | Verbo HTTP |
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


### Puntos de conexión de libro de calificaciones (lectura)

| Llamada de servicio | Punto de conexión | Verbo HTTP |
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

### Puntos de conexión de libro de calificaciones (escritura)

| Llamada de servicio | Punto de conexión | Verbo HTTP |
|-------------------------------|---------------------------------------------------|-----------|
| deleteCategory | /categories/{id} | DELETE |
| putCategory | /categories/{id} | PUT |
| deleteLineItem | /lineItems/{id} | DELETE |
| putLineItem | /lineItems/{id} | PUT |
| deleteResult | /results/{id} | DELETE |
| putResult | /results/{id} | PUT |

## Características no compatibles

Actualmente, los comportamientos siguientes no se ajustan a las especificaciones:

* [Filtrado de propiedades anidadas](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451997)
* [Selección de campos](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451998)

## Preguntas y comentarios

* Si tiene algún problema para ejecutar este ejemplo [registre un problema](https://github.com/OfficeDev/O365-EDU-SDS-AspNetMVC-Samples/issues).
* Las preguntas generales sobre el desarrollo de ASP Core deben publicarse en [Stack Overflow](https://stackoverflow.com/questions/tagged/asp.net-core). Asegúrese de que sus preguntas o comentarios incluyan la etiqueta \[asp.net-core].
* Las preguntas generales sobre el desarrollo de OneRoster deben publicarse en [IMS Global](https://www.imsglobal.org/forums/ims-glc-public-forums-and-resources/oneroster-public-forum).

## Contribuciones

Este proyecto recibe las contribuciones y las sugerencias.
La mayoría de las contribuciones requiere que acepte un Contrato de Licencia de Colaborador (CLA) donde declara que tiene el derecho, y realmente lo tiene, de otorgarnos los derechos para usar su contribución.
Para obtener más información, visite https://cla.microsoft.com.

Cuando envíe una solicitud de incorporación de cambios, un bot de CLA determinará automáticamente si necesita proporcionar un CLA y agregar el PR correctamente (por ejemplo, etiqueta, comentario).
Siga las instrucciones proporcionadas por el bot.
Solo debe hacerlo una vez en todos los repositorios que usen nuestro CLA.

Este proyecto ha adoptado el [Código de conducta de código abierto de Microsoft](https://opensource.microsoft.com/codeofconduct/).
Para obtener más información, vea [Preguntas frecuentes sobre el código de conducta](https://opensource.microsoft.com/codeofconduct/faq/)
o póngase en contacto con [opencode@microsoft.com](mailto:opencode@microsoft.com) si tiene otras preguntas o comentarios.



**Copyright (c) 2018 Microsoft. Reservados todos los derechos.**
