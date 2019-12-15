using System.Collections.Generic;
using System.Linq;
using RealEstate.Database;
using RealEstate.Database.Models;
using Microsoft.EntityFrameworkCore;
using GraphQL.Language.AST;
using System;
using System.Data.Common;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace RealEstate.DataAccess.Repositories
{
    public interface IPropertyRepository
    {
        IEnumerable<Property> GetFields(IDictionary<string, Field> subFields);
        IEnumerable<Property> GetAll();
        Property GetById(int id);
        Property Add(Property property);
    }
    public class PropertyRepository : IPropertyRepository
    {
        private readonly RealEstateContext _db;
        private readonly IConfiguration _configuration;

        public PropertyRepository(RealEstateContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public IEnumerable<Property> GetFields(IDictionary<string, Field> subFields)
        {
            string sqlQuery = BuildSqlQuery(subFields);
            var properties = Read(sqlQuery, subFields);

            return properties.AsEnumerable();
            //return _db.Properties.Select(p => new Property
            //{
            //    Name = subFields.ContainsKey("name") ? p.Name : null,
            //    Family = subFields.ContainsKey("family") ? p.Family : null,
            //    City = subFields.ContainsKey("city") ? p.City : null
            //});
        }

        private List<Property> Read(string query, IDictionary<string, Field> subFields)
        {
            var conn = _db.Database.GetDbConnection();
            conn.ConnectionString = _configuration.GetConnectionString("RealEstateDb");
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = query;
            DbDataReader reader = command.ExecuteReader();
            List<Property> properties = new List<Property>();
            Dictionary<int, List<Payment>> propertyPaymentsDict = new Dictionary<int, List<Payment>>();
            List<string> fieldNames = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i).ToLower()).ToList();
            List<Payment> payments = null;
            while (reader.Read())
            {
                int propertyId = (int)reader["Id"];
                if (!properties.Any(p => p.Id == propertyId))
                {
                    payments = new List<Payment>();
                    Property property = new Property();

                    property.Id = propertyId;

                    property.Name = CheckAndReturnValueFromReader(reader, fieldNames, "name");
                    property.City = CheckAndReturnValueFromReader(reader, fieldNames, "city");
                    property.Family = CheckAndReturnValueFromReader(reader, fieldNames, "family");
                    property.Street = CheckAndReturnValueFromReader(reader, fieldNames, "street");
                    string propertyValue = CheckAndReturnValueFromReader(reader, fieldNames, "value");
                    property.Value = propertyValue == null ? -1 : decimal.Parse(propertyValue);

                    properties.Add(property);
                    propertyPaymentsDict.Add(propertyId, payments);
                }
                Payment payment = new Payment();
                string paymtId = CheckAndReturnValueFromReader(reader, fieldNames, "paymentid");
                if (paymtId == null) continue;
                payment.Id = int.Parse(paymtId);
                payment.Id = (int)reader["paymentid"];
                string dateCreated = CheckAndReturnValueFromReader(reader, fieldNames, "datecreated");
                payment.DateCreated = dateCreated == null ? DateTime.MinValue : DateTime.Parse(dateCreated);
                string dateOverdue = CheckAndReturnValueFromReader(reader, fieldNames, "dateoverdue");
                payment.DateOverdue = dateOverdue == null ? DateTime.MinValue : DateTime.Parse(dateOverdue);
                string paid = CheckAndReturnValueFromReader(reader, fieldNames, "paid");
                payment.Paid = paid == null ? false : bool.Parse(paid);
                string paymentValue = CheckAndReturnValueFromReader(reader, fieldNames, "paymentvalue");
                payment.Value = paymentValue == null ? -1 : decimal.Parse(paymentValue);
                payments.Add(payment);
            }

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Dispose();

            if (propertyPaymentsDict.Any())
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    if (propertyPaymentsDict.ContainsKey(properties[i].Id))
                    {
                        properties[i].Payments = propertyPaymentsDict[properties[i].Id];
                    }
                }
            }

            return properties;
        }

        private string CheckAndReturnValueFromReader(DbDataReader reader, List<string> fieldNames, string fieldName)
        {
            if (fieldNames.Contains(fieldName))
            {
                return reader[fieldName].ToString();
            }
            return null;
        }

        private string BuildSqlQuery(IDictionary<string, Field> subFields)
        {
            // hard coded for now 
            // TODO: build the projection and joins based on value in subFields dictionary
            StringBuilder queryBuilder = new StringBuilder();
            List<string> projection = new List<string>
            {
                "properties.[Id]"
            };
            queryBuilder.Append("select ");
            string innerJoin = " inner join Payments as payments on payments.PropertyId = properties.Id";
            bool hasPayments = false;
            foreach (KeyValuePair<string, Field> keyValuePair in subFields)
            {
                if (keyValuePair.Key.ToLower() == "id") continue;
                if (keyValuePair.Key.ToLower() == "payments")
                {
                    projection.Add("payments.Id as PaymentId");
                    hasPayments = true;
                    Field paymentField = keyValuePair.Value;
                    foreach (Field field in paymentField.SelectionSet.Selections)
                    {
                        if (field.Name.ToLower() == "id") continue;
                        if (field.Name.ToLower() == "value")
                        {
                            projection.Add("payments.value as paymentvalue");
                        }
                        else
                        {
                            projection.Add(string.Format("payments.{0}", field.Name));
                        }
                    }
                }
                else
                {
                    projection.Add(string.Format("properties.{0}", keyValuePair.Key));
                }                
            }
            queryBuilder.Append(string.Join(", ", projection));
            queryBuilder.Append(" from Properties as properties ");
            if (hasPayments)
            {
                queryBuilder.Append(innerJoin);
            }

            return queryBuilder.ToString();
        }

        public IEnumerable<Property> GetAll()
        {
            return _db.Properties;
        }

        public Property GetById(int id)
        {
            return _db.Properties.SingleOrDefault(x => x.Id == id);
        }

        public Property Add(Property property)
        {
            _db.Properties.Add(property);
            _db.SaveChanges();
            return property;
        }
    }
}
