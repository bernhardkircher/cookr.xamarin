using System;
using Android.Content;

namespace CookR
{

	/// <summary>
	/// Contains the keys that are used to pass data between activities.
	/// </summary>
	public static class ActivityParameters
	{

		public const string RecipeId = "RECIPE_ID";

		public static void SetRecipeId(Intent intent, Recipe recipe){
			intent.PutExtra(RecipeId, recipe.Id);
		}

		public static int? GetRecipeId(Intent intent) {
			var recipeId = intent.GetIntExtra(RecipeId, -1);
			if(recipeId != -1) {
				return recipeId;
			}

			return null;
		}
	}

}

