begin tran --commit
Create table Menu (Id uniqueIdentifier Primary key DEFAULT NEWSEQUENTIALID(),
Name varchar(50) not null,
Active bit default 0
);

create table MenuSection (Id uniqueIdentifier Primary key DEFAULT NEWSEQUENTIALID(),
Name varchar(50) not null,
MenuId uniqueIdentifier not null
);

Create table MenuItem (Id uniqueIdentifier Primary key DEFAULT NEWSEQUENTIALID(),
Name varchar(50) not null,
Description varchar(200),
Section varchar(50) default '',
Price decimal (18,2) default 0,
);

Create table MenuItemsLink (Id uniqueIdentifier Primary key DEFAULT NEWSEQUENTIALID(),
MenuSectionId uniqueIdentifier not null,
MenuItemId uniqueIdentifier not null,

CONSTRAINT FK_MenuSectionLink
 FOREIGN KEY (MenuSectionId)
 REFERENCES MenuSection(Id),

 CONSTRAINT FK_MenuItemLink
 FOREIGN KEY (MenuItemId)
 REFERENCES MenuItem(Id)
);

rollback

