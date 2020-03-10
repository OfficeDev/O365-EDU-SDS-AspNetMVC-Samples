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
description: "Este projeto implementa comportamentos do OneRoster v1.1."
---

# Exemplo de provedor REST do OneRoster

Este projeto implementa comportamentos do OneRoster v1.1.

Especificamente, destina-se a requisitos para:

1. Provedor REST de Lista de Participantes do OneRoster v1.1
2. Provedor REST de Ficha de Notas do OneRoster v1.1

**Sumário**
* [Metas de exemplo](#sample-goals)
* [Pré-requisitos](#prerequisites)
* [Criar e depurar localmente](#build-and-debug-locally)
* [Implantar o exemplo no Azure](#deploy-the-sample-to-azure)
* [Sincronizar com SDS](#sync-with-sds)
* [Compreender o código](#understand-the-code)
* [Pontos de extremidade do OneRoster suportados](#supported-oneroster-endpoints)
* [Recursos sem suporte](#unsupported-features)
* [Perguntas e comentários](#questions-and-comments)
* [Colaboração](#contributing)

## Metas de exemplo

O exemplo demonstra:

* Modelos de dados compatível com [entidades do OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006)
* Os controladores de ação compatível com [pontos de extremidade de serviço do OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989)
* Middleware de autorização compatível com [Segurança Principal do OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452001)
* Geração de [pacotes CSV do OneRoster](https://www.imsglobal.org/oneroster-v11-final-csv-tables)
* Consumo de [APIs de Gerenciamento de Perfil do SDS](https://github.com/OfficeDev/O365-EDU-Tools/tree/master/SDSProfileManagementDocs) para criar e iniciar os perfis de sincronização do SDS

O OneRosterProviderDemo se baseia no Web ASP.NET Core.

## Pré-requisitos

**A implantação e execução desse exemplo requer**:

* Uma assinatura do Azure com permissões para registrar um novo aplicativo e implantação do aplicativo Web.
* Um locatário do O365 Education com o Microsoft School Data Sync habilitado
* Um dos seguintes navegadores: Edge, Internet Explorer 9, Safari 5.0.6, Firefox 5, Chrome 13 ou uma versão mais recente de um desses navegadores.
* Uma ferramenta para gerar assinaturas OAuth1, como [Postman](https://www.getpostman.com/)
* Visual Studio 2017 (qualquer edição), a [Comunidade do Visual Studio 2017](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community) está disponível gratuitamente.
* Familiaridade com C#, os aplicativos Web .NET e os serviços da Web.

## Registrar o aplicativo no Azure Active Directory

1. Entre no novo portal do Azure: [https://portal.azure.com](https://portal.azure.com/).

2. Clique em **Azure Active Directory** -> **Registros de aplicativo** -> **+Adicionar**.

   ![](Images/aad-create-app-01.png)

3. Insira um **Nome** e selecione **Aplicativo Web/API** como **Tipo de Aplicativo**.

   Insira uma **URL de Logon**: https://localhost:44344/

   ![](Images/aad-create-app-02.png)

   Clique em **Criar**.

4. Depois de concluído, o aplicativo será exibido na lista.

   ![](/Images/aad-create-app-03.png)

5. Clique nele para exibir os detalhes.

   ![](/Images/aad-create-app-04.png)

6. Clique em **Todas configurações**, se a janela de configuração não aparecer.

   * Clique em **Propriedades** e, em seguida, defina **Multilocatário** para **Sim**.

     Copie aparte o **ID do aplicativo** e, em seguida, clique em **Salvar**.

   * Clique em **Permissões necessárias**. Adicione as seguintes permissões:

     | API | Permissões delegadas |
| ------------------------------ | ---------------------------------------- |
| Microsoft Graph | Ler dados do diretório<br>Ler e gravar dados do diretório<br>Acessar o diretório como o usuário conectado<br>Ler configurações do aplicativo educacional<br>Gerenciar as configurações do aplicativo educacional |
| Windows Azure Active Directory | Entrar e ler perfil do usuário |

   * Clique em **Chaves** e, em seguida, adicione uma nova:

     ![](Images/aad-create-app-07.png)

     Clique em **Salvar** e, em seguida, copie aparte o **VALOR** da chave.

   Feche a janela de Configurações.

## Criar e depurar localmente

Esse projeto pode ser aberto com a edição do Visual Studio 2017, ou baixe e instale a edição da Comunidade para executar, criar e/ou desenvolver esse aplicativo localmente.

- [Comunidade do Visual Studio 2017](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community)

Depurar o **OneRosterProviderDemo**:

1. Configurar o **appsettings.json**.

   ![](Images/web-app-config.png)

   - **AzureAd:Authority**: "https://login.microsoftonline.com/{your-ad-tenant}.onmicrosoft.com/"
   - **AzureAd:ClientId**: usa a ID do cliente do registro do aplicativo criado anteriormente.
   - **AzureAd:ClientSecret**: usa o valor da chave do registro do aplicativo criado anteriormente.
   - **AzureDomain**: usa seu domínio de locatário do AD, que deve concordar com a entrada do **AzureAd:Authority**

2. No Console do Gerenciador de Pacotes, execute o comando `EntityFrameworkCore\Update-Database` para gerar o banco de dados inicial. Se isso causar um erro, tente executar o comando `Import-Package Microsoft.EntityFrameworkCore`.
3. Defina **OneRosterProviderDemo** como Projeto de Inicialização e pressione F5.
4. Visite `/seeds` para preencher o banco de dados com as entidades de exemplo. Lembre-se de acessar o ponto de extremidade usando HTTPS.

## Implantar o exemplo no Azure

1. Criar um Perfil de Publicação

  - Selecione **Compilar > Publicar OneRosterProviderDemo**
  - Clique em **Criar novo perfil**
  - Selecione **Serviço de Aplicativo do Microsoft Azure** e **Criar Novo**
  - Clique em **Publicar**
  - Entrar na sua conta do Azure
  - Selecione um Grupo de Recursos para a implantação ou crie um novo clicando em **Novo**
  - Selecione um Plano de Serviço de Aplicativo para a implantação ou crie um novo clicando em **Novo**
  - Clique em **Criar**

2. Selecione **Compilar > Publicar OneRosterProviderDemo**
3. Clique em **Publicar**

**Adicionar URL DE RESPOSTA ao registro de aplicativo**

1. Após a implantação, abra o grupo de recursos no portal do Azure
2. Clique no aplicativo Web.

   ![](Images/azure-web-app.png)

   Copie a URL aparte e altere o esquema para **https** e adiciona **/signin-oidc** ao final. Essa é a URL de resposta e será usada na próxima etapa.

3. Navegue até o registro do aplicativo no novo portal do Azure e, em seguida, abra as configurações do Windows.

   Adicionar a URL de resposta:

   ![](Images/aad-add-reply-url.png)

   > Observação: para depurar o exemplo localmente, certifique-se de que https://localhost:44344/signin-oidc esteja nas URLs de resposta.

4. Clique em **SALVAR**.

## Sincronizar com SDS

Há duas opções de sincronização, uma ao usar os pontos de extremidade de REST do OneRoster e outra ao carregar os [arquivos CSV compatíveis com SDS](https://support.office.com/en-us/article/CSV-files-for-School-Data-Sync-9f3c3c2b-7364-4f6e-a959-e8538feead70).

### Sincronizar com a REST do OneRoster
1. Visite sua implantação em /sds
2. Entre com uma conta de administrador em seu locatário do Education.
3. Verifique se o valor "Ponto de extremidade do OneRoster" está de acordo com a sua instância implantada
  - Observe que isso não funcionará, a menos que sua URL seja publicamente acessível
4. Clique no botão "Sincronizar via REST do OneRoster"

### Sincronizar com CSV SDS
1. Visite sua implantação em /sds
2. Entre com uma conta de administrador em seu locatário do Education.
3. Selecione seus arquivos CSV SDS
4. Clique no botão "Sincronizar via CSV SDS"

## Compreender o código

### Introdução

Esse aplicativo Web se baseia em um modelo de projeto Web do ASP.NET Core.

### Autorização

#### Pontos de extremidade do OneRoster
O arquivo `Middlewares/OAuth.cs` define um middleware que valida a assinatura de OAuth1 ou OAuth2 para cada solicitação de entrada com uma rota do OneRoster. Esse arquivo também contém o ID de cliente e o segredo codificados. Para fazer solicitações de OAuth2, primeiro visite o ponto de extremidade /token usando as credenciais de OAuth1 para obter o token de acesso. O arquivo `Startup.cs` configura o aplicativo para usar esse middleware.

#### Gerenciamento de Perfil do SDS
O arquivo `Startup.cs` configura o aplicativo para usar a biblioteca OIDC (OpenIDConnect) do .NET Core. Esse fluxo é tratado pelo `AccountController`.

### Modelos de Dados

A maioria dos modelos OneRoster tem uma classe de modelo correspondente no diretório `Modelos`. Devido às convenções de nomenclatura de idiomas, algumas delas foram renomeadas. O mapeamento de modelos é mostrado na tabela a seguir.

| Modelo do OneRoster | Modelo do EFCore |
|------------------------|-------------------|
| Base | BaseModel |
| Sessão Acadêmica | AcademicSession |
| Classe | IMSClass |
| Curso | Curso |
| Dados Demográficos | Demográfico |
| Registro | Registro |
| Item de Linha | LineItem |
| Categoria de Item de Linha | LineItemCategory |
| Org | Org |
| Recurso | Recurso |
| Resultado | Resultado |
| Usuário, Aluno, Professor | Usuário |

Foram criados modelos adicionais para dar suporte a várias necessidades:

| Modelo do EFCore | Finalidade |
|-------------------------|-------------------------------------------------|
| IMSClassAcademicSession | Tabela de relação para Classe, Termo (Sessão Acadêmica) |
| UserAgent | Tabela de relação para Usuário, Usuário |
| UserOrg | Tabela de relação para Usuário, Org |
| UserId | Formato das IDs de usuários |
| SeedData | Contém dados iniciais para preenchimento inicial do banco de dados |
| OauthNonce | Armazena valores de nonce para impedir a reutilização |

### Validação

Os requisitos de validação são definidos na [especificação do modelo do OneRoster](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452006).

Além dos atributos de validação fornecidos pelo .NET e pelo ASP.NET Core, há quatro validadores personalizados para propriedades com requisitos específicos na pasta `Validadores`.

#### Vocabulário
[Vocabularies](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452048) usados pelo OneRoster, incluindo [enumerations](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452020), são fornecidos pela classe `Vocabulary/Vocabulary`.

O vocabulário de código do assunto "SCEDv4.0" é publicado pelo [NCES](https://nces.ed.gov/forum/SCED.asp); uma conversão da planilha publicada encontra-se no arquivo `Vocabulary/sced-v4.csv`, que é analisado no momento da inicialização.

### Serialização personalizada

Todos os objetos de resposta json são gravados usando o pacote NuGet NewtonSoft JSON, para fins de correspondência com as [vinculações json indicadas](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452033).

Todas as entidades que podem ser serializadas em uma resposta de ponto de extremidade do serviço são compatíveis com dois métodos de serialização, `AsJson` e `AsJsonReference`.

**asjson** é usado quando a entidade é a entidade primária, ou um membro da coleção primária, que foi solicitado.

**AsJsonReference** é usado para expressar a entidade como uma [GUIDRef](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480452031).

### Paginação, filtragem e classificação

Os ganchos de classe `Controllers/BaseController` em [paginação, filtragem e classificação](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451994).

A filtragem e a classificação são implementadas nas classes de modelos por meio de reflexão para dar suporte aos requisitos de qualquer campo de dados que possa ser usado em filtragem ou classificação.

### Fonte de Dados

Um banco de dados do SQLite é assumido e configurado no `Startup.cs`.

Você pode criar um novo banco de dados vazio executando o comando `EntityFrameworkCore\Update-Database` no console do Gerenciador de Pacotes e propagá-lo com dados de exemplo visitando o ponto de extremidade `/seeds`.

## Pontos de extremidade do OneRoster com suporte

Há três subconjuntos de serviços do OneRoster v1.1 definidos [aqui](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451989). Dos três, os subconjuntos Lista de Participantes (somente leitura) e Ficha de Notas (leitura e gravação) estão implementados para este exemplo.

### Pontos de extremidade de Lista de Participantes (leitura)

| Chamada de serviço | Ponto de extremidade | Verbo HTTP |
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


### Pontos de extremidade de Ficha de Notas (leitura)

| Chamada de serviço | Ponto de extremidade | Verbo HTTP |
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

### Pontos de extremidade de Ficha de Notas (gravação)

| Chamada de serviço | Ponto de extremidade | Verbo HTTP |
|-------------------------------|---------------------------------------------------|-----------|
| deleteCategory | /categories/{id} | DELETE |
| putCategory | /categories/{id} | PUT |
| deleteLineItem | /lineItems/{id} | DELETE |
| putLineItem | /lineItems/{id} | PUT |
| deleteResult | /results/{id} | DELETE |
| putResult | /results/{id} | PUT |

## Recursos não suportados

No momento, os seguintes comportamentos não estão de acordo com as especificações:

* [Filtragem de propriedade aninhada](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451997)
* [Seleção de campo](https://www.imsglobal.org/oneroster-v11-final-specification#_Toc480451998)

## Perguntas e comentários

* Se você tiver problemas para executar este exemplo, [relate um problema](https://github.com/OfficeDev/O365-EDU-SDS-AspNetMVC-Samples/issues).
* Perguntas sobre o desenvolvimento do ASP Core em geral devem ser postadas no [Stack Overflow](https://stackoverflow.com/questions/tagged/asp.net-core). Verifique se suas perguntas ou comentários estão marcados com [asp.net-core].
* Perguntas sobre o desenvolvimento do OneRoster em geral devem ser postadas no [IMS Global](https://www.imsglobal.org/forums/ims-glc-public-forums-and-resources/oneroster-public-forum).

## Colaboração

Este projeto recebe e agradece as contribuições e sugestões.
A maioria das contribuições exige que você concorde com um Contrato de Licença
de Colaborador (CLA) declarando que você tem o direito a, nos conceder os direitos de uso de sua contribuição, e de fato o faz. Para saber mais, acesse https://cla.microsoft.com.

Quando você envia uma solicitação de pull, um bot de CLA determina automaticamente se você precisa fornecer um CLA e decora o PR adequadamente (por exemplo, rótulo, comentário).
Basta seguir as instruções fornecidas pelo bot.
Você só precisa fazer isso uma vez em todos os repos que usam nosso CLA.

Este projeto adotou o [Código de Conduta de Código Aberto da Microsoft](https://opensource.microsoft.com/codeofconduct/).
Para saber mais, confira as [Perguntas frequentes sobre o Código de Conduta](https://opensource.microsoft.com/codeofconduct/faq/)
ou entre em contato pelo [opencode@microsoft.com](mailto:opencode@microsoft.com) se tiver outras dúvidas ou comentários.



**Copyright (c) 2018 Microsoft. Todos os direitos reservados.**
