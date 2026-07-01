begin tran --commit
-- 1. Insert Menus
DECLARE @MenuId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Menu (Id, Name, Active)
OUTPUT inserted.Id, inserted.Name, inserted.Active
VALUES (@MenuId, 'Main Dinner Menu', 1);

-- 2. Insert Menu Sections
DECLARE @DrinksSectionId UNIQUEIDENTIFIER = NEWID();
DECLARE @SidesSectionId UNIQUEIDENTIFIER = NEWID();

INSERT INTO MenuSection (Id, Name, MenuId)
OUTPUT inserted.Id, inserted.Name, inserted.MenuId
VALUES 
(@DrinksSectionId, 'Beverages', @MenuId),
(@SidesSectionId, 'Appetizers', @MenuId);

-- 3. Insert Menu Items
DECLARE @ItemSodaId UNIQUEIDENTIFIER = NEWID();
DECLARE @ItemWaterId UNIQUEIDENTIFIER = NEWID();
DECLARE @ItemFriesId UNIQUEIDENTIFIER = NEWID();

INSERT INTO MenuItem (Id, Name, Description, Section, Price)
OUTPUT inserted.Id, inserted.Name, inserted.Description, inserted.Section, inserted.Price
VALUES
(@ItemSodaId, 'Fountain Soda', 'Cold refreshing soft drink', 'Beverages', 2.50),
(@ItemWaterId, 'Bottled Water', 'Spring water', 'Beverages', 1.99),
(@ItemFriesId, 'French Fries', 'Crispy golden potato fries', 'Appetizers', 4.99);

-- 4. Link Items to Sections
INSERT INTO MenuItemsLink (MenuSectionId, MenuItemId)
OUTPUT inserted.Id, inserted.MenuSectionId, inserted.MenuItemId
VALUES
(@DrinksSectionId, @ItemSodaId),
(@DrinksSectionId, @ItemWaterId),
(@SidesSectionId, @ItemFriesId);

rollback