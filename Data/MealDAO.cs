using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Data
{
    internal class MealDAO
    {
        private string singleConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CheckTheFridgeMealDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
        private string multiConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CheckTheFridgeMealDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True";

        public List<MealViewModel> FetchAll()
        {
            var MealList = new List<MealViewModel>();

            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                string SqlQuery = "SELECT * from [dbo].[meal]";
                SqlCommand command = new SqlCommand(SqlQuery, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var MealToIndex = new MealViewModel();
                        MealToIndex.Id = reader.GetInt32(0);
                        MealToIndex.Name = reader.GetString(1);
                        MealToIndex.Description = reader.GetString(2);
                        MealToIndex.Instructions = reader.GetString(3);
                        MealList.Add(MealToIndex);
                    }
                }
            }
            return MealList;
        }

        internal void DeleteMealfromMealDB(int id)
        {
            using (SqlConnection connection = new SqlConnection(multiConnectionString))
            {
                var sqlMealDeleteQuery = "DELETE FROM [dbo].[meal] WHERE meal_id = @id";
                var sqlIngredientDeleteQuery = "DELETE FROM [dbo].[meal_ingredient] WHERE meal_id = @id";
                SqlCommand deleteMeal = new SqlCommand(sqlMealDeleteQuery, connection);
                deleteMeal.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
                SqlCommand deleteIngredient = new SqlCommand(sqlIngredientDeleteQuery, connection);
                deleteIngredient.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
                connection.Open();
                deleteMeal.ExecuteNonQuery();
                deleteIngredient.ExecuteNonQuery();
            }
        }

        public MealViewModel FetchMeal(int mealId)
        {
            var mealToBeShown = new MealViewModel();
            var MealIngredients = new List<MealIngredientViewModel>();

            //DB Query
            using (SqlConnection connection = new SqlConnection(multiConnectionString))
            {
                //Reterive and Create Meal
                string sqlMealQuery = "SELECT * from [dbo].[meal] WHERE meal_id = @Id";
                SqlCommand getMeal = new SqlCommand(sqlMealQuery, connection);
                getMeal.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = mealId;
                connection.Open();
                SqlDataReader mealReader = getMeal.ExecuteReader();

                if (mealReader.HasRows)
                {
                    while (mealReader.Read())
                    {
                        mealToBeShown.Id = mealReader.GetInt32(0);
                        mealToBeShown.Name = mealReader.GetString(1);
                        mealToBeShown.Description = mealReader.GetString(2);
                        mealToBeShown.Instructions = mealReader.GetString(3);
                    }
                }
                MealIngredients = FetchIngredientsOfMeal(mealId);
                mealToBeShown.Ingredients = MealIngredients;
            }
            return mealToBeShown;
        }

        internal void CreateMeal(MealViewModel userCreatedMeal)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlMealCreateQuery = "INSERT INTO [dbo].[meal] VALUES (@name, @description, @instructions)";
                SqlCommand CreateMeal = new SqlCommand(sqlMealCreateQuery, connection);
                CreateMeal.Parameters.Add("@name", System.Data.SqlDbType.VarChar).Value = userCreatedMeal.Name;
                CreateMeal.Parameters.Add("@description", System.Data.SqlDbType.VarChar).Value = userCreatedMeal.Description;
                CreateMeal.Parameters.Add("@instructions", System.Data.SqlDbType.VarChar).Value = userCreatedMeal.Instructions;
                connection.Open();
                CreateMeal.ExecuteNonQuery();
            }
        }

        internal MealIngredientViewModel FetchMealIngredient(int mealIngredientID)
        {
            using (SqlConnection connection = new SqlConnection(multiConnectionString))
            {
                string sqlIngredientQuery = @"
                        SELECT m.meal_ingredient_id, m.meal_id, m.ingredient_id, i.label, m.unit_id, u.label, m.quantity
                        FROM meal_ingredient AS m
                        INNER JOIN ingredient AS i ON i.ingredient_id = m.ingredient_id
                        INNER JOIN unit AS u ON u.unit_id = m.unit_id
                        WHERE m.meal_id = @Id";
                SqlCommand command = new SqlCommand(sqlIngredientQuery, connection);
                command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = mealIngredientID;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                var ingredientToEdit = new MealIngredientViewModel();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ingredientToEdit.MealIngredientId = reader.GetInt32(0);
                        ingredientToEdit.MealId = reader.GetInt32(1);
                        ingredientToEdit.IngredientId = reader.GetInt32(2);
                        ingredientToEdit.Label = reader.GetString(3);
                        ingredientToEdit.UnitId = reader.GetInt32(4);
                        ingredientToEdit.UnitLabel = reader.GetString(5);
                        ingredientToEdit.Quantity = reader.GetDecimal(6);
                    }
                }
                return ingredientToEdit;
            }
        }

        internal void UpdateMeal(MealViewModel mealToUpdate)
        {
            //Revise dbo to reflect data in form
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                string sqlMealUpdateQuery = "UPDATE [dbo].[meal] SET name = @name, description = @description, instructions = @instructions WHERE meal_id = @id";
                SqlCommand updateMeal = new SqlCommand(sqlMealUpdateQuery, connection);
                updateMeal.Parameters.Add("@name", System.Data.SqlDbType.VarChar).Value = mealToUpdate.Name;
                updateMeal.Parameters.Add("@description", System.Data.SqlDbType.VarChar).Value = mealToUpdate.Description;
                updateMeal.Parameters.Add("@instructions", System.Data.SqlDbType.VarChar).Value = mealToUpdate.Instructions;
                updateMeal.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = mealToUpdate.Id;
                connection.Open();
                updateMeal.ExecuteNonQuery();
            }
        }

        internal void AddIngredientToMeal(MealIngredientViewModel ingredientToAdd)
        {
            if (!ingredientExistsInIngredientDb(ingredientToAdd.Label))
            {
                AddIngredientToIngredientDB(ingredientToAdd);
            }
            //add ingredient to dbo.meal_ingredient
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlAddIngredientQuery = @"
                        INSERT INTO [dbo].[meal_ingredient]
                        SELECT @meal_id, [dbo].[ingredient].ingredient_id, @unit_id, @quantity
                        FROM [dbo].[ingredient]
                        WHERE label = @label";
                SqlCommand addToMeal = new SqlCommand(sqlAddIngredientQuery, connection);
                addToMeal.Parameters.Add("@meal_id", System.Data.SqlDbType.Int).Value = ingredientToAdd.MealId;
                addToMeal.Parameters.Add("@label", System.Data.SqlDbType.VarChar).Value = ingredientToAdd.Label;
                addToMeal.Parameters.Add("@unit_id", System.Data.SqlDbType.Int).Value = ingredientToAdd.SelectedUnitId;
                addToMeal.Parameters.Add("@quantity", System.Data.SqlDbType.Decimal).Value = ingredientToAdd.Quantity;
                connection.Open();
                addToMeal.ExecuteNonQuery();
            }
        }

        internal bool ingredientExistsInIngredientDb(string ingredientLabel)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlIngredientDBOQuery = "SELECT ingredient_id FROM [dbo].[ingredient] WHERE label = @Label";
                SqlCommand ingredientDBCheck = new SqlCommand(sqlIngredientDBOQuery, connection);
                ingredientDBCheck.Parameters.Add("@Label", System.Data.SqlDbType.VarChar).Value = ingredientLabel;
                connection.Open();
                var thisIngredientObject = ingredientDBCheck.ExecuteScalar();
                if (thisIngredientObject != null)
                {
                    return true;
                }
                return false;
            }

        }

        internal bool ingredientAlreadyInMeal(MealIngredientViewModel ingredientToCheck)
        {

            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                //dbo.fridge_ingredient Query
                var sqlMealIngredientDBQuery = "SELECT ingredient_id FROM [dbo].[meal_ingredient] WHERE ingredient_id = @id";
                var ingredientID = ingredientToCheck.IngredientId;
                SqlCommand fridgeDBCheck = new SqlCommand(sqlMealIngredientDBQuery, connection);
                fridgeDBCheck.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = ingredientID;
                connection.Open();
                var fridgeCheck = fridgeDBCheck.ExecuteScalar();
                if (fridgeCheck != null)
                {
                    return true;
                }
            }
            return false;
        }

        internal void UpdateMealIngredient(MealIngredientViewModel ingredientToEdit)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlQuery = "UPDATE [dbo].[meal_ingredient] SET quantity = @quantity, unit_id = @unit WHERE meal_ingredient_id = @id";
                SqlCommand editIngredient = new SqlCommand(sqlQuery, connection);
                editIngredient.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = ingredientToEdit.MealIngredientId;
                editIngredient.Parameters.Add("@quantity", System.Data.SqlDbType.Decimal).Value = ingredientToEdit.Quantity;
                editIngredient.Parameters.Add("@unit", System.Data.SqlDbType.Int).Value = ingredientToEdit.SelectedUnitId;
                connection.Open();
                editIngredient.ExecuteNonQuery();
            }
        }

        internal void DeleteMealIngredient(int mealId)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlDeleteQuery = "DELETE FROM [dbo].[meal_ingredient] WHERE meal_ingredient_id = @Id";
                SqlCommand deleteIngredient = new SqlCommand(sqlDeleteQuery, connection);
                deleteIngredient.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = mealId;
                connection.Open();
                deleteIngredient.ExecuteNonQuery();
            }
        }

        public List<MealViewModel> PopulateMealIngredientsIntoMeals(List<MealViewModel> allMeals)
        {
            var populatedMealList = new List<MealViewModel>();

            foreach (var meal in allMeals)
            {
                var ingredientList = new List<MealIngredientViewModel>();
                ingredientList = FetchIngredientsOfMeal(meal.Id);
                meal.Ingredients = ingredientList;
                populatedMealList.Add(meal);
            }
            return populatedMealList;
        }

        internal List<MealIngredientViewModel> FetchIngredientsOfMeal(int mealId)
        {
            var ingredientList = new List<MealIngredientViewModel>();

            using (SqlConnection connection = new SqlConnection(multiConnectionString))
            {
                string sqlIngredientQuery = @"
                        SELECT m.meal_ingredient_id, m.meal_id, m.ingredient_id, i.label, m.unit_id, u.label, m.quantity
                        FROM meal_ingredient AS m
                        INNER JOIN ingredient AS i ON i.ingredient_id = m.ingredient_id
                        INNER JOIN unit AS u ON u.unit_id = m.unit_id
                        WHERE m.meal_id = @Id";
                SqlCommand getIngredient = new SqlCommand(sqlIngredientQuery, connection);
                getIngredient.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = mealId;
                connection.Open();
                SqlDataReader ingredientReader = getIngredient.ExecuteReader();

                if (ingredientReader.HasRows)
                {
                    while (ingredientReader.Read())
                    {
                        var newIngredient = new MealIngredientViewModel();
                        newIngredient.MealIngredientId = ingredientReader.GetInt32(0);
                        newIngredient.MealId = ingredientReader.GetInt32(1);
                        newIngredient.IngredientId = ingredientReader.GetInt32(2);
                        newIngredient.Label = ingredientReader.GetString(3);
                        newIngredient.UnitId = ingredientReader.GetInt32(4);
                        newIngredient.UnitLabel = ingredientReader.GetString(5);
                        newIngredient.Quantity = ingredientReader.GetDecimal(6);
                        ingredientList.Add(newIngredient);
                    }
                }
            }
            return ingredientList;
        }

        //Generic Methods

        internal void AddIngredientToIngredientDB(MealIngredientViewModel ingredientToAdd)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlAddIngredientQuery = "INSERT INTO [dbo].[ingredient] VALUES (@label)";
                SqlCommand addIngredient = new SqlCommand(sqlAddIngredientQuery, connection);
                addIngredient.Parameters.Add("@label", System.Data.SqlDbType.VarChar).Value = ingredientToAdd.Label;
                connection.Open();
                addIngredient.ExecuteScalar();
            }
        }

        internal List<Unit> FetchUnits()
        {
            var UnitList = new List<Unit>();
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                connection.Open();
                string SqlQuery = "SELECT * from [dbo].[unit]";
                SqlCommand command = new SqlCommand(SqlQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Unit UnitToIndex = new Unit();
                        UnitToIndex.unit_id = reader.GetInt32(0);
                        UnitToIndex.label = reader.GetString(1);
                        UnitList.Add(UnitToIndex);
                    }
                }
            }
            return UnitList;
        }
    }
}