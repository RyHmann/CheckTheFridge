using CheckTheFridge.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Razor.Generator;

namespace CheckTheFridge.Data
{
    public class FridgeDAO
    {
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CheckTheFridgeMealDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True";
        private string singleConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CheckTheFridgeMealDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        internal List<MealIngredientViewModel> FetchAll()
        {
            List<MealIngredientViewModel> FridgeContents = new List<MealIngredientViewModel>();
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                connection.Open();
                string sqlFridgeIngredientQuery = @"
                        SELECT f.fridge_ingredient_id, f.ingredient_id, i.label, f.unit_id, u.label, f.quantity
                        FROM fridge_ingredient AS f
                        INNER JOIN ingredient AS i ON i.ingredient_id = f.ingredient_id
                        INNER JOIN unit AS u ON u.unit_id = f.unit_id";
                SqlCommand getIngredient = new SqlCommand(sqlFridgeIngredientQuery, connection);
                SqlDataReader ingredientReader = getIngredient.ExecuteReader();
                if (ingredientReader.HasRows)
                {
                    while (ingredientReader.Read())
                    {
                        MealIngredientViewModel IngredientToIndex = new MealIngredientViewModel();
                        IngredientToIndex.FridgeIngredientId = ingredientReader.GetInt32(0);
                        IngredientToIndex.IngredientId = ingredientReader.GetInt32(1);
                        IngredientToIndex.Label = ingredientReader.GetString(2);
                        IngredientToIndex.UnitId = ingredientReader.GetInt32(3);
                        IngredientToIndex.UnitLabel = ingredientReader.GetString(4);
                        IngredientToIndex.Quantity = ingredientReader.GetDecimal(5);
                        FridgeContents.Add(IngredientToIndex);
                    }
                }
            }
            return FridgeContents;
        }

        internal MealIngredientViewModel FetchOne(int id)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                string sqlQuery = @"
                        SELECT f.fridge_ingredient_id, f.ingredient_id, i.label, f.unit_id, u.label, f.quantity
                        FROM fridge_ingredient AS f
                        INNER JOIN ingredient AS i ON i.ingredient_id = f.ingredient_id
                        INNER JOIN unit AS u ON u.unit_id = f.unit_id
                        WHERE fridge_ingredient_id = @Id";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;
                connection.Open();
                SqlDataReader ingredientReader = command.ExecuteReader();
                var ingredientToFetch = new MealIngredientViewModel();
                if (ingredientReader.HasRows)
                {
                    while (ingredientReader.Read())
                    {
                        ingredientToFetch.FridgeIngredientId = ingredientReader.GetInt32(0);
                        ingredientToFetch.IngredientId = ingredientReader.GetInt32(1);
                        ingredientToFetch.Label = ingredientReader.GetString(2);
                        ingredientToFetch.UnitId = ingredientReader.GetInt32(3);
                        ingredientToFetch.UnitLabel = ingredientReader.GetString(4);
                        ingredientToFetch.Quantity = ingredientReader.GetDecimal(5);
                    }
                }
                return ingredientToFetch;
            }
        }

        internal void UpdateIngredient(MealIngredientViewModel ingredientToEdit)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlQuery = @"
                        UPDATE [dbo].[fridge_ingredient] 
                        SET quantity = @quantity, unit_id = @unit 
                        WHERE fridge_ingredient_id = @id";
                SqlCommand editIngredient = new SqlCommand(sqlQuery, connection);
                editIngredient.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = ingredientToEdit.FridgeIngredientId;
                editIngredient.Parameters.Add("@quantity", System.Data.SqlDbType.Decimal).Value = ingredientToEdit.Quantity;
                editIngredient.Parameters.Add("@unit", System.Data.SqlDbType.Int).Value = ingredientToEdit.SelectedUnitId;
                connection.Open();
                editIngredient.ExecuteNonQuery();
            }
        }
        internal void AddIngredientToFridge(MealIngredientViewModel ingredientToAdd)
        {
            //check if ingredient exists in dbo.ingredient
            //var ingredientLabel = ingredientToAdd.Label;
            if (ingredientExistsInIngredientDb(ingredientToAdd.Label) == false)
            {
                AddIngredientToIngredientDB(ingredientToAdd);
            }
            //add ingredient to dbo.fridge_ingredient
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlAddIngredientQuery = @"
                        INSERT INTO [dbo].[fridge_ingredient]
                        SELECT [dbo].[ingredient].ingredient_id, @unit_id, @quantity
                        FROM [dbo].[ingredient]
                        WHERE label = @label";
                SqlCommand addToFridge = new SqlCommand(sqlAddIngredientQuery, connection);
                addToFridge.Parameters.Add("@label", System.Data.SqlDbType.VarChar).Value = ingredientToAdd.Label;
                addToFridge.Parameters.Add("@unit_id", System.Data.SqlDbType.Int).Value = ingredientToAdd.SelectedUnitId;
                addToFridge.Parameters.Add("@quantity", System.Data.SqlDbType.Int).Value = ingredientToAdd.Quantity;
                connection.Open();
                addToFridge.ExecuteNonQuery();
            }
        }

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

        internal void DeleteIngredient(int id)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlDeleteQuery = @"
                        DELETE FROM [dbo].[fridge_ingredient]
                        WHERE fridge_ingredient_id = @Id";
                SqlCommand deleteIngredient = new SqlCommand(sqlDeleteQuery, connection);
                deleteIngredient.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;
                connection.Open();
                deleteIngredient.ExecuteScalar();
            }
        }

        private bool ingredientExistsInIngredientDb(string ingredientLabel)
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

        //Generic Methods

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

        internal bool ingredientAlreadyInFridge(MealIngredientViewModel ingredientToCheck)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //dbo.fridge_ingredient Query
                var sqlFridgeIngredientDBOQuery = "SELECT ingredient_id FROM [dbo].[fridge_ingredient] WHERE ingredient_id = @id";
                var ingredientID = ingredientToCheck.FridgeIngredientId;
                SqlCommand fridgeDBCheck = new SqlCommand(sqlFridgeIngredientDBOQuery, connection);
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
    }
}