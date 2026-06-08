# 🍳 SmartRecipes — Painel Gastronómico Inteligente

O SmartRecipes é um ecossistema inteligente para catalogação, organização e monitorização de receitas culinárias. O projeto é composto por uma API RESTful robusta desenvolvida em C# com ASP.NET Core e Entity Framework Core (com banco de dados SQLite local) e uma interface de usuário premium e responsiva (Single Page Application) integrada diretamente no servidor de arquivos estáticos.

## Design & Identidade Visual (Light Mode Creme)

Diferente de painéis comuns, a interface do SmartRecipes foi projetada com uma estética acolhedora e sofisticada:

- **Fundo Warm Vanilla/Creme:** Um tom off-white creme amarelado acolhedor (`#FCFAF2`) que remete a livros de receitas clássicos e ambientes gastronómicos confortáveis.

- **Salgados em Verde:** Toda a sinalização visual de pratos salgados utiliza tons de Verde Culinário (`#10B981`) para transmitir frescor e ingredientes saudáveis.

- **Doces em Azul:** Sobremesas e doces recebem destaque em Azul Premium (`#3B82F6`), facilitando a rápida identificação visual.

- **Acentos em Laranja:** O laranja culinário vibrante (`#F97316`) conduz a navegação do utilizador como cor de foco principal da marca.

- **Legibilidade Máxima:** Textos e títulos utilizam fontes escuras de alto contraste para uma leitura confortável durante o preparo dos pratos.

## Funcionalidades Principais

- Gestão de Cozinheiros: Cadastro de utilizadores/chefs associados a cada receita.

- Catálogo Culinário: Lançamento completo de receitas contendo ingredientes, modo de preparo detalhado, tempo estimado e tags de classificação.

- Métricas em Tempo Real:
  - Distribuição percentual de pratos salgados e doces.
  - Tempo médio de preparo de todas as receitas do catálogo.
  - Gráfico de complexidade (receitas rápidas vs. complexas com mais de 30 minutos).
  - Distribuição percentual de receitas por categorias (Massas, Sobremesas, Bebidas, Carnes, Sopas).
  - Proporção de serviço de pratos quentes vs. frios.

- Filtros Avançados: Filtre todo o seu catálogo instantaneamente por Paladar (Doce/Salgado) ou por Temperatura de Serviço (Quente/Fria).

- Modo Dual de Conectividade: Suporta execução 100% autônoma em "Modo Demonstrativo" (In Memory) ou integrado à API C# com um clique.

## Tecnologias Utilizadas

### Backend (API)

- **Linguagem:** C# (.NET 8.0 / 9.0)
- **Framework:** ASP.NET Core Web API (Arquitetura unificada de alto desempenho no Program.cs)
- **Persistência de Dados:** Entity Framework Core
- **Banco de Dados:** SQLite (banco de dados relacional baseado em arquivo físico local)
- **Segurança e Validação:** DTOs (Data Transfer Objects) com Data Annotations para validação de entrada de dados.

### Frontend (Interface)

- **Estrutura:** HTML5 / CSS3 integrado na pasta estática `wwwroot`.
- **Estilização:** Tailwind CSS (via CDN) com configurações de tema customizadas.
- **Ícones:** Lucide Icons.

##  Estrutura do Repositório

```text
SmartRecipes/
│
├── wwwroot/             # Pasta estática servida pelo ASP.NET Core
│   └── index.html       # Interface visual do usuário (HTML/JS/Tailwind)
│
├── SmartRecipes.csproj  # Configurações do projeto e dependências .NET
└── Program.cs           # Código fonte completo unificado (Models, DTOs, Context e Controllers)
```

## Como Executar o Projeto Localmente

### Pré-requisitos

- .NET SDK instalado.
- Visual Studio 2022 ou VS Code.

### Passo a Passo de Instalação (Em Casa)

Como o projeto utiliza banco de dados SQLite, as bibliotecas do Entity Framework Core precisam de ser instaladas localmente no seu computador para compilar o código sem erros.

#### Clonar o Repositório

```bash
git clone https://github.com/CamileXavierMedina/SmartRecipes.git
cd SmartRecipes
```

#### Instalar Dependências de Banco de Dados

Abra o seu Visual Studio, vá ao menu **Ferramentas > Gerenciador de Pacotes NuGet > Console do Gerenciador de Pacotes** e execute:

```powershell
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package Microsoft.EntityFrameworkCore.Design
```

#### Compilar e Rodar

Pressione **F5** ou clique no botão **Play** no seu Visual Studio.

O servidor C# iniciará localmente.

O arquivo físico do banco de dados `smartrecipes.db` será gerado automaticamente.

As tabelas e dados iniciais de teste serão populados de forma **100% transparente** pelo nosso Seeder.

O seu painel gastronómico de cor creme abrirá imediatamente no seu navegador!

## Licença

Desenvolvido com dedicação por **Camile Xavier Medina** para fins de estudo acadêmico e portfólio profissional.

**Uso livre.**
