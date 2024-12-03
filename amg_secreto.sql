create database amg_secreto
use amg_secreto

create table participantes (
    id_part int primary key identity(1,1),
    nome varchar(50),
    email varchar(50),
	presente	varchar(100),
	mensagem	varchar(max)
);


create table presentes (
    id_present int primary key,
    id_part int foreign key references participantes(id_part),
    descri varchar(100)
);

create table sorteios (
    id_sorteio int primary key,
    id_part int foreign key references participantes(id_part),
    id_amigo_secreto int foreign key references participantes(id_part)
);

create table mensagens (
    id_mensagem int primary key,
    remetente int foreign key references participantes(id_part),
    destinatario int foreign key references participantes(id_part),
    mensagem varchar(100),
    data_envio date
);

-- Inserção de dados iniciais
insert into participantes values (1, 'joão silva', 'joao.silva@email.com');
insert into participantes values (2, 'maria oliveira', 'maria.oliveira@email.com');
insert into participantes values (3, 'carlos santos', 'carlos.santos@email.com');
insert into participantes values (4, 'ana costa', 'ana.costa@email.com');
insert into participantes values (5, 'fernanda lima', 'fernanda.lima@email.com');

insert into presentes values (1, 1, 'caixa de chocolates');
insert into presentes values (2, 2, 'livro de ficção');
insert into presentes values (3, 3, 'jogo de tabuleiro');
insert into presentes values (4, 4, 'fone de ouvido');
insert into presentes values (5, 5, 'camiseta personalizada');

insert into sorteios values (1, 1, 2);
insert into sorteios values (2, 2, 3);
insert into sorteios values (3, 3, 4);
insert into sorteios values (4, 4, 5);
insert into sorteios values (5, 5, 1);

insert into mensagens values (1, 1, 2, 'espero que goste do seu presente!', '2024-11-24');
insert into mensagens values (2, 2, 3, 'você é incrível! preparei algo especial.', '2024-11-24');
insert into mensagens values (3, 3, 4, 'feliz amigo secreto! surpresa chegando.', '2024-11-25');
insert into mensagens values (4, 4, 5, 'obrigado por tudo neste ano!', '2024-11-25');
insert into mensagens values (5, 5, 1, 'mal posso esperar para entregar seu presente!', '2024-11-26');

select * from participantes;
select * from presentes;
select * from sorteios;
select * from mensagens;

-- Procedimento para cadastro de participantes
create procedure sp_cadastrar_participante
    @nome varchar(50),
    @email varchar(50)
as
begin
    begin try
        insert into participantes (id_part, nome, email)
        values (
            (select isnull(max(id_part), 0) + 1 from participantes),
            @nome,
            @email
        );
        print 'Participante cadastrado com sucesso!';
    end try
    begin catch
        print 'Erro ao cadastrar participante: ' + ERROR_MESSAGE();
    end catch
end;

-- Procedimento para sorteio
create procedure sp_realizar_sorteio
as
begin
    declare @total_participantes int;
    declare @v_participante int;
    declare @v_amigo_secreto int;

    select @total_participantes = count(*) from participantes;

    declare participantes_cursor cursor for
        select id_part from participantes;

    open participantes_cursor;
    fetch next from participantes_cursor into @v_participante;

    while @@FETCH_STATUS = 0
    begin
        set @v_amigo_secreto = null;

        while @v_amigo_secreto is null or @v_amigo_secreto = @v_participante
        begin
            set @v_amigo_secreto = (select top 1 id_part 
                                    from participantes 
                                    where id_part != @v_participante
                                    order by newid());
        end;

        insert into sorteios (id_sorteio, id_part, id_amigo_secreto)
        values (
            (select isnull(max(id_sorteio), 0) + 1 from sorteios),
            @v_participante,
            @v_amigo_secreto
        );

        fetch next from participantes_cursor into @v_participante;
    end;

    close participantes_cursor;
    deallocate participantes_cursor;

    print 'Sorteio realizado com sucesso!';
end;

-- Procedimento para cadastro de presentes
create procedure sp_cadastrar_presente
    @id_part int,
    @descri varchar(100)
as
begin
    begin try
        insert into presentes (id_present, id_part, descri)
        values (
            (select isnull(max(id_present), 0) + 1 from presentes),
            @id_part,
            @descri
        );
        print 'Presente cadastrado com sucesso!';
    end try
    begin catch
        print 'Erro ao cadastrar presente: ' + ERROR_MESSAGE();
    end catch
end;

-- Procedimento para cadastro de mensagens
create procedure sp_enviar_mensagem
    @remetente int,
    @destinatario int,
    @mensagem nvarchar(100)
as
begin
    begin try
        insert into mensagens (id_mensagem, remetente, destinatario, mensagem, data_envio)
        values (
            (select isnull(max(id_mensagem), 0) + 1 from mensagens),
            @remetente,
            @destinatario,
            @mensagem,
            getdate()
        );
        print 'Mensagem enviada com sucesso!';
    end try
    begin catch
        print 'Erro ao enviar mensagem: ' + ERROR_MESSAGE();
    end catch
end;
