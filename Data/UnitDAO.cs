using CheckTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CheckTheFridge.Data
{
    public class UnitDAO
    {
        private string singleConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CheckTheFridgeMealDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
        private string multiConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CheckTheFridgeMealDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True";

        internal List<Unit> FetchAllUnits()
        {
            var unitList = new List<Unit>();
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlUnitQuery = "SELECT * FROM [dbo].[unit]";
                SqlCommand getUnits = new SqlCommand(sqlUnitQuery, connection);
                connection.Open();
                SqlDataReader unitReader = getUnits.ExecuteReader();

                if (unitReader.HasRows)
                {
                    while (unitReader.Read())
                    {
                        var newUnit = new Unit();
                        newUnit.label = unitReader.GetString(1);
                        unitList.Add(newUnit);
                    }
                }
            }
            return unitList;
        }

        internal void AddUnitToDb(Unit unitToAddToDb)
        {
            using (SqlConnection connection = new SqlConnection(singleConnectionString))
            {
                var sqlQueryAddToDb = "INSERT INTO [dbo].[unit] VALUES (@label)";
                SqlCommand addUnitToDb = new SqlCommand(sqlQueryAddToDb, connection);
                addUnitToDb.Parameters.Add("@label", System.Data.SqlDbType.VarChar).Value = unitToAddToDb.label;
                connection.Open();
                addUnitToDb.ExecuteNonQuery();
            }
        }
    }
}