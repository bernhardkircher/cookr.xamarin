
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;

namespace CookR
{
	[Activity (Label = "RecipeActivity")]			
	public class RecipeActivity : Activity
	{

		private Recipe recipe;

		private readonly IRecipes recipes;
		private bool isExistingRecipe;

		public RecipeActivity() : this(new Recipes()){
		}

		public RecipeActivity(IRecipes recipes){
			this.recipes = recipes;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Recipe);

			Button saveButton = FindViewById<Button> (Resource.Id.saveRecipeButton);
			Button deleteButton = FindViewById<Button> (Resource.Id.deleteRecipeButton);

			var recipeNameTextField = FindViewById<TextView> (Resource.Id.recipeNameTextField);

			int? recipeId = ActivityParameters.GetRecipeId(Intent);
			if(recipeId.HasValue) {
				recipe = recipes.GetRecipe(recipeId.Value);
				isExistingRecipe = true;
				recipeNameTextField.Text = recipe.Name;
			} else {
				recipe = new Recipe();
				isExistingRecipe = false;
			}

			deleteButton.Enabled = isExistingRecipe;


			deleteButton.Click += delegate(object sender, EventArgs e) {
				new AlertDialog.Builder(this)
					.SetMessage("Are you sure you want to delete this recipe?")
					.SetCancelable(true)
					.SetIcon(Android.Resource.Drawable.IcDialogAlert)
					.SetTitle("Delete?")
					.SetPositiveButton(Android.Resource.String.Yes, (dlgSender, dlgArgs) => {
						recipes.DeleteRecipe(recipe);
						Toast.MakeText(ApplicationContext, "Recipe deleted", ToastLength.Short).Show();
						Intent intent = new Intent(ApplicationContext, typeof(RecipesActivity));
						StartActivity(intent);
					})
					.SetNegativeButton(Android.Resource.String.No, (dlgSender, dlgArgs) => {
					})
					.Show();				
			};

			saveButton.Click += delegate {
				var newName = recipeNameTextField.Text;
				if(string.IsNullOrWhiteSpace(newName)){
					recipeNameTextField.Hint = "Please enter a recipe name";
					recipeNameTextField.Error = "Please enter a recipe name";
					Toast.MakeText(ApplicationContext, "Recipe name is empty", ToastLength.Short).Show();
					return;
				}

				var existingRecipeWithSameName = recipes.GetRecipeByName(newName);
				if(existingRecipeWithSameName != null && existingRecipeWithSameName.Id != recipe.Id){
					recipeNameTextField.Hint = "A recipe with the given name already exists";
					recipeNameTextField.Error = "A recipe with the given name already exists";
					Toast.MakeText(ApplicationContext, "A recipe already exists", ToastLength.Short).Show();
					return;
				}

				recipe.Name = newName;

				recipes.SaveRecipe(recipe);

				Toast.MakeText(ApplicationContext, "Recipe saved", ToastLength.Short).Show();

				Intent intent = new Intent(ApplicationContext, typeof(RecipesActivity));
				StartActivity(intent);
			};

		}
	}

}

