# imagem do MSSQL 2017
FROM microsoft/mssql-server-linux:2017-latest

# criar diretório dentro do contêiner SQL para arquivos de banco de dados
RUN mkdir -p /opt/mssql-scripts

# copia os arquivos de banco de dados do host para o contêiner
COPY sql/setup.sql /opt/mssql-scripts

# definir variáveis ​​de ambiente
ENV MSSQL_SA_PASSWORD=P@ssw0rd
ENV ACCEPT_EULA=Y

# executar scripts iniciais
RUN ( /opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started" \
    && sleep 15s \ 
    && /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'P@ssw0rd' -i /opt/mssql-scripts/setup.sql \
    && pkill sqlservr 

 