using CheckTheFridge.Data;
using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CheckTheFridge.Controllers
{
    public class MealsController : Controller
    {
        //Display All Meals
        public ActionResult Index()
        {
            MealDAO MealDBInterface = new MealDAO();
            List<MealViewModel> AllMeals = new List<MealViewModel>();
            AllMeals = MealDBInterface.FetchAll();
            return View("Index", AllMeals);
        }

        //Display One Meal
        public ActionResult ViewMealDetails(int id)
        {
            MealDAO MealDBInterface = new MealDAO();
            MealViewModel MealWithDetailsShown = MealDBInterface.FetchMeal(id);
            return View("Details", MealWithDetailsShown);
        }

        //Delete Meal
        public ActionResult ProcessMealToDelete(int id)
        {
            var MealDBInterface = new MealDAO();
            MealDBInterface.DeleteMealfromMealDB(id);
            List<MealViewModel> AllMeals = new List<MealViewModel>();
            AllMeals = MealDBInterface.FetchAll();
            return View("Index", AllMeals);
        }

        //Create Meal
        public ActionResult ProcessMealToCreate(MealViewModel userCreatedMeal)
        {
            var MealDBInterface = new MealDAO();
            MealDBInterface.CreateMeal(userCreatedMeal);
            List<MealViewModel> AllMeals = new List<MealViewModel>();
            AllMeals = MealDBInterface.FetchAll();
            return View("Index", AllMeals);
        }
        public ActionResult AddMeal()
        {
            var MealToAdd = new MealViewModel();
            return View("MealCreateForm", MealToAdd);
        }


        //Edit Meal
        public ActionResult UpdateMeal(MealViewModel mealToUpdate)
        {
            var MealDBInterface = new MealDAO();
            var mealToUpdateID = mealToUpdate.Id;
            var mealToDisplay = new MealViewModel();
            MealDBInterface.UpdateMeal(mealToUpdate);
            mealToDisplay = MealDBInterface.FetchMeal(mealToUpdateID);
            return View("MealForm", mealToDisplay);
        }

        public ActionResult EditMeal(int id)
        {
            var MealDBInterface = new MealDAO();
            var MealToEdit = new MealViewModel();
            MealToEdit = MealDBInterface.FetchMeal(id);
            return View("MealForm", MealToEdit);
        }

        //Remove MealIngredient from Meal
        public ActionResult RemoveMealIngredient(int id)
        {
            var MealDBInterface = new MealDAO();
            var ingredientToDelete = MealDBInterface.FetchMealIngredient(id);
            var mealId = ingredientToDelete.MealId;
            MealDBInterface.DeleteMealIngredient(id);
            var mealToDisplay = new MealViewModel();
            mealToDisplay = MealDBInterface.FetchMeal(mealId);
            return View("MealForm", mealToDisplay);
        }

        //Add MealIngredient to Meal
        public ActionResult ProcessMealIngredientToAdd(int id)
        {
            var MealDBInterface = new MealDAO();
            var mealIngredientToCreate = new MealIngredientViewModel();
            var unitList = new List<Unit>();
            unitList = MealDBInterface.FetchUnits();
            mealIngredientToCreate.MealId = id;
            mealIngredientToCreate.AvailableUnitLabels = unitList;
            return View("MealIngredientCreateForm", mealIngredientToCreate);
        }

        public ActionResult AddMealIngredientToMeal(MealIngredientViewModel ingredientToAdd)
        {
            var MealDBInterface = new MealDAO();
            var mealId = ingredientToAdd.MealId;

            if (MealDBInterface.ingredientAlreadyInMeal(ingredientToAdd))
            {
                ModelState.AddModelError("CustomError", "Ingredient already in Fridge");
                return View();
            }
            else
            {
                MealDBInterface.AddIngredientToMeal(ingredientToAdd);
            }

            var mealToDisplay = new MealViewModel();
            mealToDisplay = MealDBInterface.FetchMeal(mealId);
            return View("Mealform", mealToDisplay);
        }

        //Edit MealIngredient
        public ActionResult ProcessMealIngredientToEdit(MealIngredientViewModel ingredientToEdit)
        {
            var MealDBInterface = new MealDAO();
            var mealId = ingredientToEdit.MealId;
            MealDBInterface.UpdateMealIngredient(ingredientToEdit);
            var mealToDisplay = new MealViewModel();
            mealToDisplay = MealDBInterface.FetchMeal(mealId);
            return View("MealForm", mealToDisplay);
        }

        //Edit MealIngredient
        public ActionResult ProcessIngredientToEdit(int id)
        {
            var MealDBInterface = new MealDAO();
            var mealIngredientToEdit = new MealIngredientViewModel();
            mealIngredientToEdit = MealDBInterface.FetchMealIngredient(id);
            var unitLabels = new List<Unit>();
            unitLabels = MealDBInterface.FetchUnits();
            mealIngredientToEdit.AvailableUnitLabels = unitLabels;
            return View("MealIngredientForm", mealIngredientToEdit);
        }
    }
}