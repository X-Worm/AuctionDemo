//using System;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.Linq;
//using System.Web;

//namespace AuctionDemo.Services
//{
//    public static class FieldLevelSelectionService
//    {
//        public static object CreateDataShapedObject<T>(this IQueryable<T> source , string fields) 
//        {
//            List<string> lstOfFields = new List<string>();
//            // Get list of needed fields
//            if (fields != null)
//            {
//                lstOfFields = fields.ToLower().Split(',').ToList();
//            }

//            // if lstOfFields doesnt contain fields return source without changing
//            if (!lstOfFields.Any())
//            {
//                return source;
//            }
//            else
//            {
//                // dynamicly create object with needed fields
//                ExpandoObject objectToReturn = new ExpandoObject();
//                foreach (var field in lstOfFields)
//                {
//                    var fieldValue = source.GetType().GetProperty(field, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
//                        .GetValue(source, null);

//                    ((IDictionary<string, object>)objectToReturn).Add(field, fieldValue);
//                }
//                return objectToReturn;
//            }
//        }

//    }
//}