# WebAPI - Gestão de Cidades e Alunos

Esta é uma API desenvolvida em C# com ASP.NET Core para gerenciamento de cidades e alunos. O sistema permite a importação em lote de todas as cidades do brasil (2026) através de um arquivo CSV para um banco de dados MySQL, operações de CRUD para cidades, e a gestão de alunos com suporte a upload e conversão de fotos para Base64. A infraestrutura é totalmente orquestrada via Docker.  

Desenvolvido com o objetivo de resolver o trabalho proposto pela disciplina Linguagens de Programação - 1° Bim. 2026.

## 🚀 Tecnologias Utilizadas

* **C# / ASP.NET Core:** Framework principal da Web API.
* **MySQL:** Banco de dados relacional para persistência das cidades e alunos.
* **Docker:** Containerização e orquestração da API e do Banco de Dados.
* **Swagger/OpenAPI:** Documentação interativa e testes de endpoints.

## ⚙️ Funcionalidades

O projeto foi dividido em três partes principais:

### Parte 1: Gestão e Importação de Cidades
* Importação de dados de cidades via upload de arquivo `.csv` diretamente para o banco de dados.
* Suporte a processamento em lote (parâmetro `usarLote`) para arquivos extensos.
* Operações de CRUD (Create, Read, Update, Delete) para as cidades.
* Filtros e agregações: contagem total de cidades, listagem de UFs e filtro de cidades por estado específico.

### Parte 2: Gestão de Alunos (Desafio)
* Cadastro e consulta de entidades de alunos vinculados às cidades.
* Endpoint de upload para receber arquivos de imagem (foto do aluno) via `multipart/form-data`.
* Endpoint de visualização que retorna a foto armazenada do aluno codificada em string Base64.

### Parte 3: Documentação
* A API implementa o padrão OpenAPI, disponibilizando a interface do Swagger para consulta e teste de todos os contratos e rotas.

## 🛣️ Endpoints da API

Abaixo está o resumo das rotas expostas pela aplicação.

### 🏙️ Cidades

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| `POST` | `/importar` | Importa cidades de um arquivo `.csv` (Suporta `usarLote=true`). |
| `GET` | `/api/cidades` | Retorna todas as cidades cadastradas. |
| `POST` | `/api/cidades` | Insere manualmente uma nova cidade. |
| `GET` | `/api/cidades/{id}` | Retorna os detalhes de uma cidade específica pelo ID. |
| `PUT` | `/api/cidades/{id}` | Altera os dados de uma cidade existente. |
| `DELETE` | `/api/cidades/{id}` | Exclui uma cidade pelo ID. |
| `GET` | `/total` | Retorna a quantidade total de cidades cadastradas. |
| `GET` | `/estados` | Retorna a lista de todos os estados (UFs) cadastrados. |
| `GET` | `/estado/{uf}` | Retorna uma lista de cidades filtrada pela sigla do estado. |

### 🧑‍🎓 Alunos

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| `POST` | `/Alunos` | Cria um novo registro de aluno. |
| `GET` | `/Alunos/{id}` | Consulta um registro de aluno pelo ID. |
| `POST` | `/Alunos/{id}/foto` | Recebe e armazena a foto do aluno via form-data. |
| `GET` | `/Alunos/{id}/foto` | Retorna a foto do aluno codificada em formato Base64. |

## 📦 Estrutura do Arquivo CSV Esperado

**Nota:** Um arquivo de exemplo (`cidade.csv`) encontra-se na raiz deste projeto para testes de importação.

Para o endpoint `/importar`, o arquivo `.csv` deve conter a seguinte estrutura de colunas (com cabeçalho):

1.  **CidadeId**: Identificador único da cidade (Int).
2.  **Nome**: Nome da cidade (String).
3.  **Sigla**: Sigla do estado / UF (String).
4.  **IBGEMunicipio**: Código IBGE da cidade (Int).
5.  **Latitude**: Latitude da cidade (Double/Decimal).
6.  **Longitude**: Longitude da cidade (Double/Decimal).

## 🛠️ Como Executar o Projeto

1. **Clone o repositório:**  
   ```bash
   git clone https://github.com/mateus-sm/WebAPI.git

Toda a infraestrutura do projeto, incluindo a inicialização e criação das tabelas do MySQL, está automatizada com Docker.

2. **Suba os contêineres:**  
Certifique-se de ter o Docker Desktop (ou Docker Engine) rodando na sua máquina.
No terminal, na raiz do projeto (onde está o arquivo docker-compose.yml), execute:  
   ```bash
    docker-compose up -d --build

O Docker fará o download da imagem do MySQL, criará o banco de dados cidades_db via init.sql e iniciará os serviços.

3. **Acesse a API:**  
Com os contêineres em execução, rode o projeto C# via Visual Studio ou método de preferencia.  
Abra o navegador e acesse a URL mapeada na rota /doc.  
Output bash ao rodar o projeto:  
     ```bash
     info: Microsoft.Hosting.Lifetime[14]
          Now listening on: http://localhost:5182
     info: Microsoft.Hosting.Lifetime[0]
          Application started. Press Ctrl+C to shut down.
     info: Microsoft.Hosting.Lifetime[0]
          Hosting environment: Development
     info: Microsoft.Hosting.Lifetime[0]
          Content root path: C:\WebAPI

No caso acima o acesso seria em: http://localhost:5182/doc
