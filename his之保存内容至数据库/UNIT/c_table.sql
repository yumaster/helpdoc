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
--目录
--drop table mulu
create table mulu(
id int IDENTITY(1,1) not null,
pid int null,
node hierarchyid not null,
name nvarchar(300) not null,--目录名称
url varchar(100) null,
addTime datetime not null		constraint[DF_MULU_ADDTIME]default(getdate()),
reviseTime datetime not null	constraint[DF_MULU_REVISETIME]default(getdate()),
isdel bit not null				constraint[DF_MULU_ISDEL]default(0)
)
GO
--创建广度优先索引
create clustered index ICX_MULU_PID_NODE on mulu(pid,node)
GO
ALTER TABLE mulu ADD CONSTRAINT [PK_MULU] PRIMARY KEY (id)--主键
GO
--drop table log_mulu
create table log_mulu(
id int IDENTITY(1,1) not null CONSTRAINT [PK_LOG_MULU] PRIMARY KEY,
mid int not null,
url varchar(100) not null,
addTime datetime not null constraint[DF_LOG_MULU_ADDTIME]default(getdate()),
)