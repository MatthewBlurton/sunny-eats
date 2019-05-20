INSERT INTO [dbo].[Ingredient] ([IngredientName])
VALUES
	('white sugar'),
	('butter'),
	('eggs'),
	('vanilla essence'),
	('plain flour'),
	('baking powder'),
	('milk');

INSERT INTO [dbo].[Category] ([CategoryName])
VALUES
	('Cake');

INSERT INTO [dbo].[Recipe] ([RecipeName], [RecipeDescription], [RecipePrepTime], [CategoryId], [RecipeNumOfServes])
VALUES
	('Simple Vanilla Cake',
	'A really great simple cake batter that can be used for cakes or muffins. Easy enough for kids to make and decorate. Retrieved from: http://allrecipes.com.au/recipe/4679/simple-vanilla-cake.aspx',
	'00:50:00', 1, 12);

INSERT INTO [dbo].[RecipeReceipt] ([RecipeId], [IngredientID], [RecipeReceiptQuantity])
VALUES
	(1, 1, '1 cup'),
	(1, 2, '125g'),
	(1, 3, '2'),
	(1, 4, '2 tsp'),
	(1, 5, '1 1/2 cups'),
	(1, 6, '1 1/3 tsp'),
	(1, 7, '1/2 cup');

INSERT INTO [dbo].[RecipeStep] ([StepNumber], [RecipeId], [StepDescription])
VALUES
	(1, 1, 'Preheat oven to 180 degrees C. Grease and flour a 23cm x 23cm cake pan or line a muffin pan with paper liners.'),
	(2, 1, 'In a medium bowl, cream together the sugar and butter. Beat in the eggs, one at a time, then stir in the vanilla essence.'),
	(3, 1, 'Combine flour and baking powder, add to the creamed mixture and mix well. Finally stir in the milk until mixture is smooth. Pour or spoon into the prepared pan.'),
	(4, 1, 'Bake in preheated oven for 30 to 40 minutes. For cupcakes, bake 20 to 25 minutes. Cake is done when it springs back to the touch.');