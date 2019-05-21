DROP TABLE IF EXISTS [RecipeStep];
DROP TABLE IF EXISTS [RecipeIngredient];
DROP TABLE IF EXISTS [Ingredient];
DROP TABLE IF EXISTS [Recipe];
DROP TABLE IF EXISTS [Category];

CREATE TABLE [Category]
(
    [ID]            INT IDENTITY(1, 1) PRIMARY KEY,
    [Name]          NVARCHAR(50)  UNIQUE NOT NULL
);

CREATE TABLE [Recipe]
(
    [ID]                INT IDENTITY(1, 1) PRIMARY KEY,
    [Name]              NVARCHAR(50) NOT NULL,
    [Description]       NVARCHAR(255) NULL,
    [PrepTime]          NVARCHAR(50) NULL,
    [CategoryID]        INT NULL,
    [NumberOfServes]    NVARCHAR(50) NULL,
    [Cal_kJ_PerServe]   NVARCHAR(50) NULL,
    CONSTRAINT FK_RecipeToCategory FOREIGN KEY ([CategoryID])
        REFERENCES [Category]([ID])
);

CREATE TABLE [Ingredient]
(
    [ID]            INT IDENTITY(1, 1) PRIMARY KEY,
    [Name]          NVARCHAR(50) NOT NULL,
    [Description]   NVARCHAR(255) NULL
);

CREATE TABLE [RecipeIngredient]
(
	[ID]						INT IDENTITY(1, 1) PRIMARY KEY,
    [RecipeID]					INT NOT NULL,
    [IngredientID]				INT NOT NULL,
    [Quantity]					NVARCHAR(100) NOT NULL,
	[IngredientAppend]			NVARCHAR(50) NULL,
    CONSTRAINT FK_RecipeIngredientToRecipe FOREIGN KEY ([RecipeID])
        REFERENCES [Recipe]([ID])
		ON DELETE CASCADE,
    CONSTRAINT FK_RecipeIngredientToIngredient FOREIGN KEY ([IngredientID])
        REFERENCES [Ingredient]([ID])
);

CREATE TABLE [RecipeStep]
(
    [ID]            INT IDENTITY(1, 1) PRIMARY KEY,
    [RecipeID]      INT NOT NULL,
    [Number]        INT NOT NULL,
    [Description]   NVARCHAR(1000) NOT NULL,
    CONSTRAINT FK_RecipeStepToRecipe FOREIGN KEY ([RecipeID])
        REFERENCES [Recipe]([ID])
		ON DELETE CASCADE
);

INSERT INTO [Category] ([Name])
VALUES	('Cake'),
		('Desert');

INSERT INTO [Recipe] ([Name], [Description], [PrepTime], [CategoryID], [NumberOfServes], [Cal_kJ_PerServe])
VALUES	('Lemon drizzle cake', 'https://www.bbcgoodfood.com/recipes/4942/lemon-drizzle-cake', '15 min', 1, '10 Slices', '399 kcal'),
		('Lemon & poppyseed cupcakes', 'https://www.bbcgoodfood.com/recipes/470636/lemon-and-poppyseed-cupcakes', '22 min', 2, '12', '529 kcal');

INSERT INTO [Ingredient] ([Name])
VALUES	('butter'),			-- 1
		('sugar'),			-- 2
		('egg'),			-- 3
		('flour'),			-- 4
		('lemon'),			-- 5
		('poppy seed'),		-- 6
		('yogurt'),			-- 7
		('food colouring'),	-- 8
		('sprinkles');		-- 9

INSERT INTO [RecipeIngredient] ([RecipeID], [IngredientID], [Quantity], [IngredientAppend])
VALUES	(1, 1, '225g unsalted ', ', softened'),	-- Recipe 1
		(1, 2, '225g caster ', ''),
		(1, 3, '4 ', 's'),
		(1, 4, '225g self-raising ', ''),
		(1, 5, 'finely grated zest 1 ', ''),
		(1, 5, 'juice 1 1/2 ', 's'),
		(1, 2, '85g caster ', ''),
		(2, 3, '225g self-raising', ''),		-- Recipe 2
		(2, 2, '175g golden caster ', ''),
		(2, 5, 'zest 2 ', 's'),
		(2, 6, '1 tbsp ', ', toasted'),
		(2, 3, '3 ', 's'),
		(2, 7, '100g natural ', ''),
		(2, 1, '175g ', ', melted and cooled a little'),
		(2, 1, '225g ', ', softened'),
		(2, 2, '400g icing ', ', sifted'),
		(2, 5, 'juice 1 ', ''),
		(2, 8, 'few drops yellow ', ''),
		(2, 9, 'icing flowers or yellow ', ', to decorate');

INSERT INTO [RecipeStep] ([RecipeID], [Number], [Description])
VALUES	(1, 1, 'Heat oven to 180C/fan 160C/gas 4.'), -- Recipe 1
		(1, 2, 'Beat together 225g softened unsalted butter and 225g caster sugar until pale and creamy, then add 4 eggs, one at a time, slowly mixing through.'),
		(1, 3, 'Sift in 225g self-raising flour, then add the finely grated zest of 1 lemon and mix until well combined.'),
		(1, 4, 'Line a loaf tin (8 x 21cm) with greaseproof paper, then spoon in the mixture and level the top with a spoon.'),
		(1, 5, 'Bake for 45-50 mins until a thin skewer inserted into the centre of the cake comes out clean.'),
		(1, 6, 'While the cake is cooling in its tin, mix together the juice of 1 ½ lemons and 85g caster sugar to make the drizzle.'),
		(1, 7, 'Prick the warm cake all over with a skewer or fork, then pour over the drizzle – the juice will sink in and the sugar will form a lovely, crisp topping.'),
		(1, 8, 'Leave in the tin until completely cool, then remove and serve. Will keep in an airtight container for 3-4 days, or freeze for up to 1 month.'),
		-- Recipe 2
		(2, 1, 'Heat oven to 180C/160C fan/gas 4 and line a 12-hole muffin tin with cupcake or muffin cases. Mix the flour, sugar, lemon zest and poppy seeds together in a large mixing bowl. Beat the eggs into the yogurt, then tip this into the dry ingredients with the melted butter. Mix together with a wooden spoon or whisk until lump-free, then divide between the cases. Bake for 20-22 mins until a skewer poked in comes out clean – the cakes will be quite pale on top still. Cool for 5 mins in the tin, then carefully lift onto a wire rack to finish cooling.'),
		(2, 2, 'To ice, beat the softened butter until really soft in a large bowl, then gradually beat in the icing sugar and lemon juice. Stir in enough food colouring for a pale lemon colour, then spoon the icing into a piping bag with a large star nozzle.'),
		(2, 3, 'Ice one cake at a time, holding the piping bag almost upright with the nozzle about 1cm from the surface of the cake. Pipe one spiral of icing around the edge, then pause to break the flow before moving the nozzle towards the centre slightly and piping a second, smaller spiral that continues until there are no gaps in the centre. Slightly ‘dot’ the nozzle into the icing as you stop squeezing to finish neatly. Repeat to cover all the cakes, then top with sugar decorations or scatter with sprinkles.');