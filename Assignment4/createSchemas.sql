DROP TABLE IF EXISTS TagTask;
DROP TABLE IF EXISTS tag;
DROP TABLE IF EXISTS task;
DROP TABLE IF EXISTS developer;

CREATE TABLE developer(
	id SERIAL PRIMARY KEY,
	title VARCHAR(100),
	email VARCHAR(100)
);


CREATE TABLE tag(
	id SERIAL PRIMARY KEY,
    name VARCHAR(50)
);


CREATE TABLE task(
	id SERIAL PRIMARY KEY,
	title VARCHAR (100),
    developerId SERIAL REFERENCES developer(id),
    description varchar(65535),
    state VARCHAR(20)
);

--many to many reference task to tag

CREATE TABLE TagTask(
	tagId SERIAL REFERENCES tag(id), 
	taskId SERIAL REFERENCES task(id)
);

--select * from TagTask