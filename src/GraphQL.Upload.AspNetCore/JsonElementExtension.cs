using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace GraphQL.Upload.AspNetCore
{
    public static class JsonElementExtension
    {
        public static Dictionary<string, object> ToDictionnary(this JsonElement element)
        {
            return element.EnumerateObject()
                .ToDictionary(
                    jsonProperty => jsonProperty.Name, 
                    jsonProperty => GetRealFromType(jsonProperty.Value)
                );
        }

        private static object GetRealFromType(JsonElement jsonPropertyValue)
        {
            switch (jsonPropertyValue.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.False:
                case JsonValueKind.True:
                    return jsonPropertyValue.GetBoolean();
                case JsonValueKind.Number:
                    var decimalValue = jsonPropertyValue.GetDecimal();

                    if (decimalValue % 1 != 0)
                    {
                        try
                        {
                            return jsonPropertyValue.GetDouble();
                        }
                        catch (FormatException)
                        {
                        }

                        return decimalValue;
                    }

                    return jsonPropertyValue.GetInt32();
                case JsonValueKind.String:
                    try
                    {
                        return jsonPropertyValue.GetGuid();
                    }
                    catch (FormatException)
                    {
                    }
                    
                    try
                    {
                        return jsonPropertyValue.GetDateTime();
                    }
                    catch (FormatException)
                    {
                    }
                    
                    return jsonPropertyValue.GetString();
                case JsonValueKind.Object:
                    return jsonPropertyValue.EnumerateObject().ToDictionary(
                        value => value.Name, 
                        value => GetRealFromType(value.Value)
                    );
                case JsonValueKind.Array:
                    return jsonPropertyValue.EnumerateArray().Select(GetRealFromType);
            }
            return jsonPropertyValue;
        }
    }
}