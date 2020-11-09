using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Data
{
    public class ListsDAO
    {
        internal List<MealViewModel> CalculateAvailableMeals(List<MealViewModel> allMeals, Fridge userFridge)
        {
            var potentialMealList = new List<MealViewModel>();

            //go through meals and get ingredients
            foreach (var meal in allMeals)
            {
                if (AllMealIngredientsInFridge(meal, userFridge) == true)
                {
                    potentialMealList.Add(meal);
                }
            }
            //see if meal ingredients are in fridge
            //see if meal ingredient quantity is > than fridge ingredient
            return potentialMealList;
        }

        internal bool AllMealIngredientsInFridge(MealViewModel mealToTest, Fridge fridgeToTest)
        {
            var fridgeIngredients = new HashSet<int>();
            var mealIngredients = new HashSet<int>();

            //Populate meal ingredients
            foreach (var ingredient in mealToTest.Ingredients)
            {
                mealIngredients.Add(ingredient.IngredientId);
            }
            //Populate fridge ingredients
            foreach (var ingredient in fridgeToTest.Contents)
            {
                fridgeIngredients.Add(ingredient.IngredientId);
            }
            //Test if all meal ingredients are present in fridge ingredients
            if (mealIngredients.IsProperSubsetOf(fridgeIngredients))
            {
                return true;
            }
            return false;
        }
    }
}