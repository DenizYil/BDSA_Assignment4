drop table if exists TagTask;
drop table if exists tag;
drop table if exists task;
drop table if exists developer;
create table developer(
	id real,
	Title varchar(100),
	Email varchar(100),
	--TaskID real,
	primary key(id)
);
create table tag(
	id real,
	Name varchar(50),
	primary key(id)
);
create table task(
	id real,
	Title varchar(100),
	DeveloperID real,
	Description varchar(65535),
	state varchar(20),
	--, TaskID real
	primary key(id),
	foreign key(DeveloperID) references developer(id)
);

--many to many reference task to tag
create table TagTask(
	tagID real,
	taskID real,
	foreign key(tagID) references tag(id),
	foreign key(taskID) references task(id)
);

select * from TagTask