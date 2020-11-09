using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Data
{
    public class IngredientDAO
    {
        private string singleConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CheckTheFridgeMealDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

        internal List<Ingredient> FetchAllIngredients()
        {
            var ingredientList = new List<Ingredient>();
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlIngredientQuery = "SELECT * FROM [dbo].[ingredient]";
                SqlCommand getIngredients = new SqlCommand(sqlIngredientQuery, connection);
                connection.Open();
                SqlDataReader ingredientReader = getIngredients.ExecuteReader();

                if (ingredientReader.HasRows)
                {
                    while (ingredientReader.Read())
                    {
                        var newIngredient = new Ingredient();
                        newIngredient.Id = ingredientReader.GetInt32(0);
                        newIngredient.Name = ingredientReader.GetString(1);
                        ingredientList.Add(newIngredient);
                    }
                }
            }
            return ingredientList;
        }

        internal void AddIngredientToDb(Ingredient ingredientToAddToDb)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlQueryAddToDb = "INSERT INTO [dbo].[ingredient] VALUES (@label)";
                SqlCommand addIngredientToDb = new SqlCommand(sqlQueryAddToDb, connection);
                addIngredientToDb.Parameters.Add("@label", System.Data.SqlDbType.VarChar).Value = ingredientToAddToDb.Name;
                connection.Open();
                addIngredientToDb.ExecuteNonQuery();
            }
        }
    }
}