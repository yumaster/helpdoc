create database helpdb 
on primary
(
    name='helpdb',
    filename='D:\db\helpdb.mdf',
    size=5MB,
    maxsize=unlimited,
    filegrowth=15%
)
log on
(
    name='helpdb_log',
    filename='D:\db\helpdb_log.ldf',
    size=1MB,
	maxsize=1GB,
    filegrowth=1MB
)
GO
--Ŀ¼
--drop table mulu
create table mulu(
id int IDENTITY(1,1) not null,
pid int null,
node hierarchyid not null,
name nvarchar(300) not null,--Ŀ¼����
url varchar(100) null,
addTime datetime not null		constraint[DF_MULU_ADDTIME]default(getdate()),
reviseTime datetime not null	constraint[DF_MULU_REVISETIME]default(getdate()),
isdel bit not null				constraint[DF_MULU_ISDEL]default(0)
)
GO
--���������������
create clustered index ICX_MULU_PID_NODE on mulu(pid,node)
GO
ALTER TABLE mulu ADD CONSTRAINT [PK_MULU] PRIMARY KEY (id)--����
GO
--drop table log_mulu
create table log_mulu(
id int IDENTITY(1,1) not null CONSTRAINT [PK_LOG_MULU] PRIMARY KEY,
mid int not null,
url varchar(100) not null,
addTime datetime not null constraint[DF_LOG_MULU_ADDTIME]default(getdate()),
)