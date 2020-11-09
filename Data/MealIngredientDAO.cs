using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Data
{
    public class MealIngredientDAO
    {
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CheckTheFridgeMealDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True";

        internal MealIngredient FetchOne(object id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM [dbo].[fridge_ingredient] WHERE fridge_ingredient_id = @Id";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                var ingredientToEdit = new MealIngredient();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ingredientToEdit.Id = reader.GetInt32(0);
                        var ingredientNameId = reader.GetInt32(1);
                        var ingredientUnitId = reader.GetInt32(2);
                        ingredientToEdit.Quantity = reader.GetDecimal(3);

                        var ingredientName = AssignIngredientName(ingredientNameId, connection);
                        ingredientToEdit.Name = ingredientName;
                        //Assign Ingredient Unit
                        var ingredientUnitName = AssignIngredientUnit(ingredientUnitId, connection);
                        ingredientToEdit.Unit = ingredientUnitName;
                    }
                }
                return ingredientToEdit;
            }
        }

        private string AssignIngredientName(int ingredientNameIndex, SqlConnection connection)
        {
            string sqlIngredientQuery = "SELECT label from [dbo].[ingredient] WHERE ingredient_id = @Id";
            SqlCommand getIngredient = new SqlCommand(sqlIngredientQuery, connection);
            getIngredient.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = ingredientNameIndex;
            var ingredientLabel = getIngredient.ExecuteScalar();
            return ingredientLabel.ToString();
        }

        private string AssignIngredientUnit(int ingredientUnitIndex, SqlConnection connection)
        {
            string sqlUnitQuery = "SELECT label from [dbo].[unit] WHERE unit_id = @Id";
            SqlCommand getUnit = new SqlCommand(sqlUnitQuery, connection);
            getUnit.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = ingredientUnitIndex;
            var unitLabel = getUnit.ExecuteScalar();
            return unitLabel.ToString();
        }
    }
}