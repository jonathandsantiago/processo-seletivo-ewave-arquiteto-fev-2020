## Avaliação Arquitetura - Jonathan Dias Campos Santiago

Backend - desenvolvido em C# .NET com ASP.NET Core 3.0, Entity Framework, autenticação e autorização OAuth2 - JWT e Docker
Docker 
	- Api
	- Banco de dados SQL Server - 2017
	- RabbitMq
Projeto estruturado com a metodologia DDD e TDD 
distribuito por camadas:
	■ Apresentação;
	■ Repositorio;
	■ Serviço;
	■ Dominio;
	■ Testes;
	
Frontend - Angular (Typescript) utilizando programação Reativa (RxJS) e gerenciamento de Estado (Akita/NgRX), Material designer e Docker
Docker 
	- Front
	
# Anotações
A primeira execução do projeto backend, no docker será buildado a execução do script de estruturação do banco de dados,
por padrão já irá vir incluso alguns produtos e usuários.
Usuários com o tipo garçom e cozinha.
Usuários:
	Login: garcom
	Password: garom
	--
	Login: cozinha
	Password: cozinha	


# Sobre o sistema
Projecto de controle de comandas:
O sistema é executado conforme o tipo do cadastro do usuário.
Garçom:
	■ Home
		Listando todas as comandas em aberto
	■ Nova comanda 
		Campos
			- Garçom logado
			- Produto
			- Quantidade
		Lista dos produtos adicionados	
		Botões
			Adicionar pedido
			Salvar
Cozinha:
		■ Home
			Kaban listando todas as comandas em aberto, sendo separado por novos pedidos, pedidos em execução e finalizados.
		Ações:
			- Novos pedidos: irá conter a ação Inicar e cancelar;
			- Pedidos execução: irá conter a ação Finalizar e cancelar;
			- Pedidos execução: Não contem ações.
Cada novos pedidos adicionados por usuário do tipo garçom api irá enviar uma mensagen para a cozinha carregando o 'Json'
do novo pedido criado.